﻿using FarmsToFeedUs.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FarmsToFeedUs.ImportService.Services
{
    public class FarmDataToFarmService : IFarmDataToFarmService
    {
        public FarmDataToFarmService(IPostcodeService postcodeService, IHttpClientFactory httpClientFactory, IGeohashService geohashService, ILogger<FarmDataToFarmService> logger)
        {
            PostcodeService = postcodeService;
            GeohashService = geohashService;
            Logger = logger;

            HttpClient = httpClientFactory.CreateClient();
            HttpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(GetType().Namespace);
        }

        private IPostcodeService PostcodeService { get; }
        public HttpClient HttpClient { get; }
        private IGeohashService GeohashService { get; }
        private ILogger Logger { get; }

        public async Task<Farm> MakeFarmFromFarmDataAsync(FarmData farmData)
        {
            PostcodeResult? postcodeLookup = await GetPostcodeResultAsync(farmData);

            return new Farm
            {
                Name = farmData.Name ?? "-- missing --",
                Town = farmData.Town ?? postcodeLookup?.Parish,
                County = farmData.County ?? postcodeLookup?.AdminCounty,
                Postcode = postcodeLookup?.Postcode ?? farmData.Postcode,
                Latitude = postcodeLookup?.Latitude,
                Longitude = postcodeLookup?.Longitude,
                GeoHash = ParseGeoHash(postcodeLookup),
                Website = await ParseWebsiteAsync(farmData),
                Instagram = ParseInstagram(farmData),
                Facebook = await ParseFacebookAsync(farmData)
            };
        }

        private async Task<PostcodeResult?> GetPostcodeResultAsync(FarmData farmData)
        {
            PostcodeResult? postcodeLookup = null;

            if (!string.IsNullOrWhiteSpace(farmData.Postcode))
                postcodeLookup = await PostcodeService.GetPostcodeInfoAsync(farmData.Postcode);

            if (postcodeLookup == null && !string.IsNullOrWhiteSpace(farmData.Town))
                postcodeLookup = await PostcodeService.GetPlaceAsync(farmData.Town);

            if (postcodeLookup == null && !string.IsNullOrWhiteSpace(farmData.County))
                postcodeLookup = await PostcodeService.GetPlaceAsync(farmData.County);

            return postcodeLookup;
        }

        private long? ParseGeoHash(PostcodeResult? postcodeLookup)
        {
            if (postcodeLookup?.Latitude != null && postcodeLookup?.Longitude != null)
            {
                return GeohashService.Encode(postcodeLookup.Latitude.Value, postcodeLookup.Longitude.Value);
            }

            return null;
        }

        private async Task<string?> ParseWebsiteAsync(FarmData farmData)
        {
            if (string.IsNullOrWhiteSpace(farmData.SocialMedia))
                return null;

            var parts = farmData.SocialMedia.Split(' ', ',');
            string? address = parts.FirstOrDefault(p => p.Contains(".") && !p.Contains("facebook.com", StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrWhiteSpace(address))
                return null;

            string httpsAddress = address;

            if (!address.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                httpsAddress = $"https://{address}";

            try
            {
                var response = await HttpClient.GetAsync(httpsAddress);
                response.EnsureSuccessStatusCode();

                return httpsAddress;
            }
            catch (Exception ex)
            {
                Logger.LogDebug(ex, $"Failed to query website on URL \"{httpsAddress}\" for farm \"{farmData.Name}\"");
            }

            if (!address.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                string httpAddress = $"http://{address}";

                try
                {
                    var response = await HttpClient.GetAsync(httpAddress);
                    response.EnsureSuccessStatusCode();

                    return httpAddress;
                }
                catch (Exception ex)
                {
                    Logger.LogDebug(ex, $"Failed to query website on URL \"{httpAddress}\" for farm \"{farmData.Name}\"");
                }
            }

            return null;
        }

        private string? ParseInstagram(FarmData farmData)
        {
            if (string.IsNullOrWhiteSpace(farmData.SocialMedia))
                return null;

            var parts = farmData.SocialMedia.Split(' ', ',');
            string? handle = parts.FirstOrDefault(p => p.Contains("@"));

            if (string.IsNullOrWhiteSpace(handle) || !handle.StartsWith("@"))
                return null;

            return handle;
        }

        private async Task<string?> ParseFacebookAsync(FarmData farmData)
        {
            if (string.IsNullOrWhiteSpace(farmData.SocialMedia))
                return null;

            var parts = farmData.SocialMedia.Split(' ', ',');
            string? url = parts.FirstOrDefault(p => p.Contains("facebook.com", StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrWhiteSpace(url))
                return null;

            if (!url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                url = $"https://{url}";

            try
            {
                var response = await HttpClient.GetAsync(url);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return url;
            }
            catch (Exception)
            {
            }

            return null;
        }
    }
}
