using Amazon;
using Amazon.Lambda.Core;
using FarmsToFeedUs.Common;
using FarmsToFeedUs.Data;
using FarmsToFeedUs.ImportService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FarmsToFeedUs.ImportService
{
    public class Function : LambdaFunctionBase
    {
        // Lambda constructor
        public Function() : base()
        {
        }

        // Lambda / Test constructor
        public Function(RegionEndpoint regionEndpoint, EnvironmentEnum environment) : base(regionEndpoint, environment)
        {
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<IPostcodeService, PostcodeIOHttpClient>();

            services.AddSingleton<IFarmDataService, FarmDataService>();
            services.AddSingleton<IImportService, Services.ImportService>();

            services.AddData(Environment);
        }

        public async Task FunctionHandlerAsync()
        {
            Logger.LogInformation($"Beginning import service");

            var service = ServiceProvider.GetRequiredService<IImportService>();
            await service.BeginAsync();

            Logger.LogInformation("Completed import service");
        }
    }
}
