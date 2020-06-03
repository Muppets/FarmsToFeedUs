using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FarmsToFeedUs.Common
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonLogging(this IServiceCollection services, EnvironmentEnum environment)
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

                if (environment == EnvironmentEnum.Dev)
                    logging.AddDebug();

                logging.SetMinimumLevel(environment == EnvironmentEnum.Live ? LogLevel.Information : LogLevel.Debug);
            });

        }
    }
}
