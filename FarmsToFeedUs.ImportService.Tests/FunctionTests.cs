using Amazon;
using FarmsToFeedUs.Data;
using System.Threading.Tasks;
using Xunit;

namespace FarmsToFeedUs.ImportService.Tests
{
    public class FunctionTests
    {
        [Fact]
        public async Task TestFunction()
        {
            var function = new Function(RegionEndpoint.EUWest1, EnvironmentEnum.Localhost);
            await function.FunctionHandlerAsync();
        }
    }
}
