﻿using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.DependencyInjection;

namespace FarmsToFeedUs.Data
{
    public static class ServiceCollectionExtensions
    {
        public static void AddData(this IServiceCollection services, EnvironmentEnum environment)
        {
            // Use dev for localhost dynamo tables
            if (environment == EnvironmentEnum.Localhost)
                environment = EnvironmentEnum.Dev;

            AWSConfigsDynamoDB.Context.TableNamePrefix = $"{environment}-";

            services.AddAWSService<IAmazonDynamoDB>();

            services.AddTransient<IDynamoDBContext, DynamoDBContext>();

            services.AddSingleton<IFarmRepository, FarmDynamoDbRepository>();
        }
    }
}