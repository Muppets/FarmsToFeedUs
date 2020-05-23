using Amazon;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using FarmsToFeedUs.Common;
using FarmsToFeedUs.Data;
using FarmsToFeedUs.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            Logger.LogInformation($"Beginning list function");

            var respository = ServiceProvider.GetRequiredService<IFarmRepository>();

            var list = await respository.ListAllAsync();
            var farmModels = list.Select(f => GetFarmModel(f));

            Logger.LogInformation("Completed list function");

            return CreateApiResponse(farmModels);
        }

        private FarmModel GetFarmModel(Farm f)
        {
            return new FarmModel
            {
                Name = f.Name,
                Town = f.Town,
                County = f.County,
                Postcode = f.Postcode,
                Latitude = f.Latitude,
                Longitude = f.Longitude,
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
