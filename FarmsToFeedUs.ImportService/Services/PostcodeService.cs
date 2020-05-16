using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FarmsToFeedUs.ImportService.Services
{
    public class PostcodeIOHttpClient : IPostcodeService
    {
        public PostcodeIOHttpClient(HttpClient client)
        {
            Client = client;
            Client.BaseAddress = new Uri("https://api.postcodes.io/");
        }

        private HttpClient Client { get; }

        public async Task<PostcodeResult> GetPostcodeAsync(string postcode)
        {
            var responseMessage = await Client.GetAsync($"postcodes/{postcode}");
            responseMessage.EnsureSuccessStatusCode();

            var json = await responseMessage.Content.ReadAsStringAsync();
            var message = JsonSerializer.Deserialize<PostcodeMessage>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (message.Status != (long)HttpStatusCode.OK)
                throw new Exception("Failed to complete postcode look up");

            return message.Result;
        }

        private class PostcodeMessage
        {
            public long Status { get; set; }
            public PostcodeResult Result { get; set; } = new PostcodeResult();
        }
    }
}
