using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace FarmsToFeedUs.Data.Tests
{
    public class PostcodeServiceTests
    {
        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetPostcodeInfoAsync_ByPostcode()
        {
            var services = new ServiceCollection();
            services.AddData(Common.EnvironmentEnum.Dev);
            var serviceProvider = services.BuildServiceProvider();

            var postcodeService = serviceProvider.GetRequiredService<IPostcodeService>();
            var postcodeInfo = await postcodeService.GetPostcodeInfoAsync("GU341JT");

            Assert.NotNull(postcodeInfo);
            Assert.Equal("GU34 1JT", postcodeInfo!.Postcode);
            Assert.Equal("Alton", postcodeInfo.Parish);
            Assert.Equal(51.146933, postcodeInfo.Latitude);
            Assert.Equal(-0.982808, postcodeInfo.Longitude);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetPostcodeInfoAsync_ByLatLong()
        {
            var services = new ServiceCollection();
            services.AddData(Common.EnvironmentEnum.Dev);
            var serviceProvider = services.BuildServiceProvider();

            var postcodeService = serviceProvider.GetRequiredService<IPostcodeService>();
            var postcodeInfo = await postcodeService.GetPostcodeInfoAsync(51.146933, -0.982808);

            Assert.NotNull(postcodeInfo);
            Assert.Equal("GU34 1JT", postcodeInfo!.Postcode);
            Assert.Equal("Alton", postcodeInfo.Parish);
            Assert.Equal(51.146933, postcodeInfo.Latitude);
            Assert.Equal(-0.982808, postcodeInfo.Longitude);
        }
    }
}
