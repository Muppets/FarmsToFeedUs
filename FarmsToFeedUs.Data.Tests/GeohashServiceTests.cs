using Xunit;

namespace FarmsToFeedUs.Data.Tests
{
    public class GeohashServiceTests
    {
        [Theory]
        [Trait("Category", "UnitTest")]
        [InlineData(51.146933, -0.982808, 2162354424914814)]
        [InlineData(51.132707, 0.536566, 3663602073312830)]
        public void Encode(double latitude, double longitude, long result)
        {
            GeohashService service = new GeohashService();
            Assert.Equal(result, service.Encode(latitude, longitude));
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public void GetRanges()
        {
            double latitude = 51.146933;
            double longitude = -0.982808;

            GeohashService service = new GeohashService();
            var range = service.GetRanges(latitude, longitude);

            Assert.Equal(2162357000000000, range[0].Min);
            Assert.Equal(2162357999999999, range[0].Max);
            Assert.Equal(2162359000000000, range[1].Min);
            Assert.Equal(2162359999999999, range[1].Max);
        }
    }
}
