using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using FarmsToFeedUs.Common;
using Microsoft.Extensions.DependencyInjection;

namespace FarmsToFeedUs.Data
{
    public static class ServiceCollectionExtensions
    {
        public static void AddData(this IServiceCollection services, EnvironmentEnum environment)
        {
            AWSConfigsDynamoDB.Context.TableNamePrefix = $"{environment}-";

            services.AddHttpClient<IPostcodeService, PostcodeIOHttpClient>();

            services.AddAWSService<IAmazonDynamoDB>();

            services.AddTransient<IDynamoDBContext, DynamoDBContext>();

            services.AddSingleton<IFarmRepository, FarmDynamoDbRepository>();
        }
    }
}
