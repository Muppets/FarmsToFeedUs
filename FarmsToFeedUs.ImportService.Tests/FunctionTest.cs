using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Lambda.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace FarmsToFeedUs.ImportService.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async Task TestFunction()
        {
            var context = new TestLambdaContext();
            var services = new ServiceCollection();

            services.AddAWSService<IAmazonDynamoDB>();
            services.AddDefaultAWSOptions(new AWSOptions { Region = RegionEndpoint.EUWest1 });

            AWSConfigsDynamoDB.Context.TableNamePrefix = "Dev-";

            var function = new Function(services);
            await function.FunctionHandlerAsync(context);

            var testLogger = context.Logger as TestLambdaLogger;
            Assert.Contains("Completed import service", testLogger?.Buffer.ToString());
        }
    }
}
