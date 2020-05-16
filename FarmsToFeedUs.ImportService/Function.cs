using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using FarmsToFeedUs.ImportService.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FarmsToFeedUs.ImportService
{
    public class Function
    {
        // dotnet lambda deploy-function FarmsToFeedUs.ImportService --msbuild-parameters "/p:PublishReadyToRun=true --self-contained false"

        private IServiceProvider ServiceProvider { get; }

        public Function()
        {
            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<IPostcodeService, PostcodeIOHttpClient>();

            services.AddSingleton<IFarmDataService, FarmDataService>();

            services.AddAWSService<IAmazonDynamoDB>();
        }

        public async Task FunctionHandler(ILambdaContext context)
        {
            context.Logger.LogLine($"Beginning import service");

            var service = ServiceProvider.GetRequiredService<IImportService>();
            await service.BeginAsync();

            context.Logger.LogLine("Completed import service");
        }
    }
}
