using Amazon;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using FarmsToFeedUs.Common;
using FarmsToFeedUs.Data;
using FarmsToFeedUs.Shared;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FarmsToFeedUs.Api
{
    public class ListFunction : LambdaFunctionBase
    {
        // Lambda constructor
        public ListFunction() : base()
        {
        }

        // Lambda / Test constructor
        public ListFunction(RegionEndpoint regionEndpoint, EnvironmentEnum environment) : base(regionEndpoint, environment)
        {
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddData(Environment);
        }

        public async Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayProxyRequest request)
        {
            if (!double.TryParse(request.QueryStringParameters["lat"], out var latitude) || !double.TryParse(request.QueryStringParameters["long"], out var longitude))
                return CreateValidationResponse("Missing valid lat or long parameters");

            var response = await ListFarmsByLatLongAsync(latitude, longitude);

            return CreateApiResponse(response);
        }

        private async Task<List<FarmModel>> ListFarmsByLatLongAsync(double latitude, double longitude)
        {
            var postcodeService = ServiceProvider.GetRequiredService<IPostcodeService>();
            var postcodeInfo = postcodeService.GetPostcodeInfoAsync(latitude, longitude);

            var respository = ServiceProvider.GetRequiredService<IFarmRepository>();

            var list = await respository.ListByLatLongAsync(latitude, longitude);
            var farmModels = list.Select(f => GetFarmModel(f));

            return farmModels.ToList();
        }

        private FarmModel GetFarmModel(Farm f)
        {
            string? instagramUrl = null;

            if (!string.IsNullOrWhiteSpace(f.Instagram))
                instagramUrl = $"https://www.instagram.com/{f.Instagram[1..]}/";

            return new FarmModel
            {
                Name = f.Name,
                Town = f.Town,
                County = f.County,
                Postcode = f.Postcode,
                Latitude = f.Latitude,
                Longitude = f.Longitude,
                WebsiteUrl = f.Website,
                InstagramUrl = instagramUrl,
                FacebookUrl = f.Facebook,
            };
        }

        private static APIGatewayProxyResponse CreateValidationResponse(string message)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = JsonSerializer.Serialize(message),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        private static APIGatewayProxyResponse CreateApiResponse(object response)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonSerializer.Serialize(response),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}
