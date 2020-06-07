using FarmsToFeedUs.Common;
using FarmsToFeedUs.Data;
using FarmsToFeedUs.ImportService.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace FarmsToFeedUs.ImportService.Tests
{
    public class FarmDataToFarmServiceTests
    {
        public FarmDataToFarmServiceTests()
        {
            var services = new ServiceCollection();
            services.AddData(EnvironmentEnum.Dev);
            services.AddSingleton<IFarmDataService, FarmDataService>();
            services.AddSingleton<IFarmDataToFarmService, FarmDataToFarmService>();
            services.AddCommonLogging(EnvironmentEnum.Dev);

            ServiceProvider = services.BuildServiceProvider();
            FarmDataToFarmService = ServiceProvider.GetRequiredService<IFarmDataToFarmService>();
        }

        private IServiceProvider ServiceProvider { get; }
        private IFarmDataToFarmService FarmDataToFarmService { get; }

        //[Fact]
        //[Trait("Category", "Integration")]
        //public async Task MakeFarmFromFarmDataAsync_TestAll()
        //{
        //    var farmDataService = ServiceProvider.GetRequiredService<IFarmDataService>();

        //    var farmDatas = await farmDataService.GetFarmDataAsync();

        //    foreach (var farmData in farmDatas)
        //    {
        //        var farm = await FarmDataToFarmService.MakeFarmFromFarmDataAsync(farmData);

        //        Assert.Equal(farmData.Name, farm.Name);
        //    }
        //}

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task MakeFarmFromFarmDataAsync_Website_HttpOnly()
        {
            var farmData = new FarmData { SocialMedia = @"www.lowerclopton.co.uk, Facebook: Lower Clopton Farm Shop and Cafe, @lowercloptonfarmshoppton" };
            var farm = await FarmDataToFarmService.MakeFarmFromFarmDataAsync(farmData);

            Assert.Equal("http://www.lowerclopton.co.uk", farm.Website);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task MakeFarmFromFarmDataAsync_Website_Https()
        {
            var farmData = new FarmData { SocialMedia = @"www.scottiescoffee.co.uk" };
            var farm = await FarmDataToFarmService.MakeFarmFromFarmDataAsync(farmData);

            Assert.Equal("https://www.scottiescoffee.co.uk", farm.Website);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task MakeFarmFromFarmDataAsync_Instagram()
        {
            var farmData = new FarmData { SocialMedia = @"www.learnlandleisure.com, @hillhousefarmdorking, www.facebook.com/hillhousefarmdorking" };
            var farm = await FarmDataToFarmService.MakeFarmFromFarmDataAsync(farmData);

            Assert.Equal("@hillhousefarmdorking", farm.Instagram);
        }
    }
}
