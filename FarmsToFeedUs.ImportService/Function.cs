using Amazon.Lambda.Core;
using FarmsToFeedUs.Data;
using FarmsToFeedUs.ImportService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FarmsToFeedUs.ImportService
{
    public class Function
    {
        // dotnet lambda deploy-function FarmsToFeedUs.ImportService --msbuild-parameters "/p:PublishReadyToRun=true --self-contained false"
        // dotnet lambda deploy-serverless

        private IServiceProvider ServiceProvider { get; }

        public Function() : this(new ServiceCollection())
        {
        }

        public Function(IServiceCollection services)
        {
            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();
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
                logging.AddDebug();
                logging.SetMinimumLevel(LogLevel.Debug);
            });

            services.AddHttpClient<IPostcodeService, PostcodeIOHttpClient>();

            services.AddSingleton<IFarmDataService, FarmDataService>();
            services.AddSingleton<IImportService, Services.ImportService>();

            services.AddData();
        }

        public async Task FunctionHandlerAsync(ILambdaContext context)
        {
            context.Logger.LogLine($"Beginning import service");

            var service = ServiceProvider.GetRequiredService<IImportService>();
            await service.BeginAsync();

            context.Logger.LogLine("Completed import service");
        }
    }
}
