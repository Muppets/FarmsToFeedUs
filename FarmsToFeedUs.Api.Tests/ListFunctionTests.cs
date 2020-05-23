using Amazon;
using FarmsToFeedUs.Common;
using FarmsToFeedUs.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace FarmsToFeedUs.Api.Tests
{
    public class ListFunctionTests
    {
        [Fact]
        public async Task TestFunction()
        {
            var function = new ListFunction(RegionEndpoint.EUWest1, EnvironmentEnum.Localhost);
            var response = await function.FunctionHandlerAsync(new Amazon.Lambda.APIGatewayEvents.APIGatewayProxyRequest { });

            var repository = function.ServiceProvider.GetRequiredService<IFarmRepository>();

            var farms = JsonSerializer.Deserialize<List<Farm>>(response.Body);
            var dbFarms = await repository.ListAllAsync();

            Assert.Equal(dbFarms.Count, farms.Count);
            Assert.Equal(dbFarms[0].Name, farms[0].Name);
            Assert.Equal(dbFarms[0].Town, farms[0].Town);
            Assert.Equal(dbFarms[0].County, farms[0].County);
            Assert.Equal(dbFarms[0].Postcode, farms[0].Postcode);
        }
    }
}
