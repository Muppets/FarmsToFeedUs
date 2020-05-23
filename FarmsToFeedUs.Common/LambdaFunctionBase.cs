using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace FarmsToFeedUs.Common
{
    public abstract class LambdaFunctionBase
    {
        // dotnet lambda deploy-serverless --msbuild-parameters "/p:PublishReadyToRun=true --self-contained false" - Only works on linux
        // dotnet lambda deploy-serverless

        protected IServiceProvider ServiceProvider { get; }

        protected RegionEndpoint RegionEndpoint { get; }

        protected EnvironmentEnum Environment { get; }

        protected ILogger Logger { get; }

        protected virtual void ConfigureServices(IServiceCollection services) { }

        // Lambda constructor
        public LambdaFunctionBase() : this(GetRegionEndpoint(), GetEnvironment())
        {
            AWSSDKHandler.RegisterXRayForAllServices();
        }

        // Lambda / Test constructor
        public LambdaFunctionBase(RegionEndpoint regionEndpoint, EnvironmentEnum environment)
        {
            RegionEndpoint = regionEndpoint;
            Environment = environment;

            var services = new ServiceCollection();

            ConfigureServicesInternal(services);

            ServiceProvider = services.BuildServiceProvider();

            Logger = ServiceProvider.GetRequiredService<ILogger<LambdaFunctionBase>>();

            Logger.LogInformation("BuildServiceProvider completed");
        }

        private void ConfigureServicesInternal(IServiceCollection services)
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

            services.AddDefaultAWSOptions(new AWSOptions
            {
                Region = RegionEndpoint
            });

            ConfigureServices(services);
        }

        private static RegionEndpoint GetRegionEndpoint()
        {
            return RegionEndpoint.GetBySystemName(System.Environment.GetEnvironmentVariable("AWS_REGION"));
        }

        private static EnvironmentEnum GetEnvironment()
        {
            return (EnvironmentEnum)Enum.Parse(typeof(EnvironmentEnum), System.Environment.GetEnvironmentVariable("Environment") ?? "");
        }
    }
}
