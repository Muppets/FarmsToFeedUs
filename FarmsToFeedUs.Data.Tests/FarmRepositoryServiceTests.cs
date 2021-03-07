using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace FarmsToFeedUs.Data.Tests
{
    public class FarmRepositoryServiceTests
    {
        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetPostcodeInfoAsync_ByLatLong()
        {
            var services = new ServiceCollection();
            services.AddData(Common.EnvironmentEnum.Dev);
            services.AddDefaultAWSOptions(new AWSOptions
            {
                Region = RegionEndpoint.EUWest1
            });
            var serviceProvider = services.BuildServiceProvider();

            var farmRepository = serviceProvider.GetRequiredService<IFarmRepository>();
            var farms = await farmRepository.ListByLatLongAsync(51.146933, -0.982808);

            Assert.NotNull(farms);
            Assert.Equal(10, farms!.Count);
            Assert.Equal("Alton", farms[0].Name);
        }
    }
}
