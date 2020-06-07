using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FarmsToFeedUs.Data
{
    public class PostcodeIOHttpClient : IPostcodeService
    {
        public PostcodeIOHttpClient(HttpClient client, ILogger<PostcodeIOHttpClient> logger)
        {
            Client = client;
            Logger = logger;

            Client.BaseAddress = new Uri("https://api.postcodes.io/");
        }

        private HttpClient Client { get; }

        private ILogger Logger { get; }

        public async Task<PostcodeResult?> GetPostcodeInfoAsync(string postcode)
        {
            try
            {
                var responseMessage = await Client.GetAsync($"postcodes/{postcode}");
                responseMessage.EnsureSuccessStatusCode();

                var json = await responseMessage.Content.ReadAsStringAsync();
                var message = JsonSerializer.Deserialize<PostcodeMessage>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (message.Status != HttpStatusCode.OK)
                    throw new Exception($"Failed to complete postcode look up status {message.Status} returned: {json}");

                return message.Result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to get postcode details for \"{postcode}\"");
            }

            return null;
        }

        public async Task<PostcodeResult?> GetPostcodeInfoAsync(double latitude, double longitude)
        {
            try
            {
                var responseMessage = await Client.GetAsync($"postcodes?lon={longitude}&lat={latitude}");
                responseMessage.EnsureSuccessStatusCode();

                var json = await responseMessage.Content.ReadAsStringAsync();
                var message = JsonSerializer.Deserialize<PostcodesMessage>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (message.Status != HttpStatusCode.OK)
                    throw new Exception($"Failed to complete postcode look up status {message.Status} returned: {json}");

                return message.Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to get postcode details for latitude \"{latitude}\" and longitude \"{longitude}\"");
            }

            return null;
        }

        public async Task<PostcodeResult?> GetPlaceAsync(string place)
        {
            try
            {
                var responseMessage = await Client.GetAsync($"places?q={WebUtility.UrlEncode(place)}");
                responseMessage.EnsureSuccessStatusCode();

                var json = await responseMessage.Content.ReadAsStringAsync();
                var message = JsonSerializer.Deserialize<PlaceMessage>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (message.Status != HttpStatusCode.OK)
                    throw new Exception($"Failed to complete place look up status {message.Status} returned: {json}");

                return message.Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to get place details for \"{place}\"");
            }

            return null;
        }

        private class PostcodeMessage
        {
            public HttpStatusCode? Status { get; set; }
            public PostcodeResult Result { get; set; } = new PostcodeResult();
        }

        private class PostcodesMessage
        {
            public HttpStatusCode? Status { get; set; }
            public List<PostcodeResult> Result { get; set; } = new List<PostcodeResult>();
        }

        private class PlaceMessage
        {
            public HttpStatusCode? Status { get; set; }
            public List<PostcodeResult> Result { get; set; } = new List<PostcodeResult>();
        }
    }
}
