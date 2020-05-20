using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.DependencyInjection;

namespace FarmsToFeedUs.Data
{
    public static class ServiceCollectionExtensions
    {
        public static void AddData(this IServiceCollection services)
        {
            services.AddAWSService<IAmazonDynamoDB>();

            services.AddTransient<IDynamoDBContext, DynamoDBContext>();

            services.AddSingleton<IFarmRepository, FarmDynamoDbRepository>();
        }
    }
}
