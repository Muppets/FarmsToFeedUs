using FarmsToFeedUs.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FarmsToFeedUs.ImportService.Services
{
    public class FarmDataToFarmService : IFarmDataToFarmService
    {
        public FarmDataToFarmService(IPostcodeService postcodeService, IHttpClientFactory httpClientFactory, ILogger<FarmDataToFarmService> logger)
        {
            PostcodeService = postcodeService;
            Logger = logger;

            HttpClient = httpClientFactory.CreateClient();
            HttpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(GetType().Namespace);
        }

        private IPostcodeService PostcodeService { get; }
        public HttpClient HttpClient { get; }
        private ILogger Logger { get; }

        public async Task<Farm> MakeFarmFromFarmDataAsync(FarmData farmData)
        {
            PostcodeResult? postcodeLookup = await GetPostcodeResultAsync(farmData);

            return new Farm
            {
                Name = farmData.Name ?? "-- missing --",
                Town = farmData.Town ?? postcodeLookup?.Parish,
                County = farmData.County ?? postcodeLookup?.AdminCounty,
                Postcode = farmData.Postcode,
                Latitude = postcodeLookup?.Latitude,
                Longitude = postcodeLookup?.Longitude,
                Website = await ParseWebsiteAsync(farmData),
                Instagram = await ParseInstagramAsync(farmData),
                Facebook = await ParseFacebookAsync(farmData)
            };
        }

        private async Task<PostcodeResult?> GetPostcodeResultAsync(FarmData farmData)
        {
            PostcodeResult? postcodeLookup = null;

            if (!string.IsNullOrWhiteSpace(farmData.Postcode))
                postcodeLookup = await PostcodeService.GetPostcodeAsync(farmData.Postcode);

            if (postcodeLookup == null && !string.IsNullOrWhiteSpace(farmData.Town))
                postcodeLookup = await PostcodeService.GetPlaceAsync(farmData.Town);

            if (postcodeLookup == null && !string.IsNullOrWhiteSpace(farmData.County))
                postcodeLookup = await PostcodeService.GetPlaceAsync(farmData.County);

            return postcodeLookup;
        }

        private async Task<string?> ParseWebsiteAsync(FarmData farmData)
        {
            if (string.IsNullOrEmpty(farmData.SocialMedia))
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
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return httpsAddress;
            }
            catch (Exception)
            {
            }

            if (!address.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                string httpAddress = $"http://{address}";

                try
                {
                    var response = await HttpClient.GetAsync(httpAddress);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return httpAddress;
                }
                catch (Exception)
                {
                }
            }

            return null;
        }

        private async Task<string?> ParseInstagramAsync(FarmData farmData)
        {
            if (string.IsNullOrEmpty(farmData.SocialMedia))
                return null;

            var parts = farmData.SocialMedia.Split(' ', ',');
            string? handle = parts.FirstOrDefault(p => p.Contains("@"));

            if (string.IsNullOrWhiteSpace(handle) || !handle.StartsWith("@"))
                return null;

            try
            {
                var handleWithoutAt = handle.Substring(1);
                var response = await HttpClient.GetAsync($"https://www.instagram.com/{handleWithoutAt}/");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return handle;
            }
            catch (Exception)
            {
            }

            return null;
        }

        private async Task<string?> ParseFacebookAsync(FarmData farmData)
        {
            if (string.IsNullOrEmpty(farmData.SocialMedia))
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
