using FarmsToFeedUs.ImportService.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace FarmsToFeedUs.ImportService.Tests
{
    public class FarmDataServiceTests
    {
        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetFarmDataAsync()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            var serviceProvider = services.BuildServiceProvider();

            var farmDataService = new FarmDataService(serviceProvider.GetRequiredService<IHttpClientFactory>());
            var farmData = await farmDataService.GetFarmDataAsync();

            Assert.True(farmData.Count > 200);
            Assert.False(farmData.GroupBy(f => f.Name).Where(g => g.Count() > 1).Any());
            Assert.True(farmData.All(f => !string.IsNullOrWhiteSpace(f.Name)));
        }
    }
}
