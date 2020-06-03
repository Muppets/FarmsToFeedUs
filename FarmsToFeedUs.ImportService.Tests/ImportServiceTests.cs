using Amazon;
using Amazon.Extensions.NETCore.Setup;
using FarmsToFeedUs.Data;
using FarmsToFeedUs.ImportService.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace FarmsToFeedUs.ImportService.Tests
{
    public class ImportServiceTests
    {
        public ImportServiceTests()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            services.AddSingleton<IImportService, Services.ImportService>();
            services.AddHttpClient<IPostcodeService, PostcodeIOHttpClient>();
            services.AddSingleton<IFarmDataService, FarmDataService>();
            services.AddSingleton<IFarmDataToFarmService, FarmDataToFarmService>();
            services.AddData(Common.EnvironmentEnum.Dev);

            services.AddDefaultAWSOptions(new AWSOptions
            {
                Region = RegionEndpoint.EUWest1
            });

            ServiceProvider = services.BuildServiceProvider();
            ImportService = ServiceProvider.GetRequiredService<IImportService>();
        }

        private IServiceProvider ServiceProvider { get; }
        private IImportService ImportService { get; }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task BeginAsync()
        {
            await ImportService.BeginAsync();
        }

        //[Fact]
        //[Trait("Category", "DataJob")]
        //public async Task BeginAsync_ForceUpdateAllRecords()
        //{
        //    await ImportService.BeginAsync(forceUpdate: true);
        //}
    }
}
