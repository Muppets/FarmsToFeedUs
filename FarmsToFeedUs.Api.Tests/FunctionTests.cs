using Amazon;
using FarmsToFeedUs.Common;
using System.Threading.Tasks;
using Xunit;

namespace FarmsToFeedUs.Api.Tests
{
    public class FunctionTests
    {
        [Fact]
        public async Task TestFunction()
        {
            var function = new ListFunction(RegionEndpoint.EUWest1, EnvironmentEnum.Localhost);
            await function.FunctionHandlerAsync(new Amazon.Lambda.APIGatewayEvents.APIGatewayProxyRequest { });
        }
    }
}
