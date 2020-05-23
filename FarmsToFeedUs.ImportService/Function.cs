using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Lambda.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using FarmsToFeedUs.Data;
using FarmsToFeedUs.ImportService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FarmsToFeedUs.ImportService
{
    public class Function
    {
        // dotnet lambda deploy-function FarmsToFeedUs.ImportService --msbuild-parameters "/p:PublishReadyToRun=true --self-contained false"
        // dotnet lambda deploy-serverless

        private IServiceProvider ServiceProvider { get; }

        private RegionEndpoint RegionEndpoint { get; }

        private EnvironmentEnum Environment { get; }

        private ILogger Logger { get; }

        // Lambda constructor
        public Function() : this(
            RegionEndpoint.GetBySystemName(System.Environment.GetEnvironmentVariable("AWS_REGION")),
            (EnvironmentEnum)Enum.Parse(typeof(EnvironmentEnum), System.Environment.GetEnvironmentVariable("Environment") ?? ""))
        {
            AWSSDKHandler.RegisterXRayForAllServices();
        }

        // Lambda / Test constructor
        public Function(RegionEndpoint regionEndpoint, EnvironmentEnum environment)
        {
            RegionEndpoint = regionEndpoint;
            Environment = environment;

            var services = new ServiceCollection();

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            Logger = ServiceProvider.GetRequiredService<ILogger<Function>>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging((logging) =>
            {
                logging.AddLambdaLogger(new LambdaLoggerOptions
                {
                    IncludeCategory = true,
                    IncludeLogLevel = true,
                    IncludeNewline = true,
                    IncludeEventId = true,
                    IncludeException = true
                });

                if (Environment == EnvironmentEnum.Localhost)
                    logging.AddDebug();

                logging.SetMinimumLevel(Environment == EnvironmentEnum.Live ? LogLevel.Information : LogLevel.Debug);
            });

            services.AddHttpClient<IPostcodeService, PostcodeIOHttpClient>();

            services.AddSingleton<IFarmDataService, FarmDataService>();
            services.AddSingleton<IImportService, Services.ImportService>();

            services.AddDefaultAWSOptions(new AWSOptions
            {
                Region = RegionEndpoint
            });

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
