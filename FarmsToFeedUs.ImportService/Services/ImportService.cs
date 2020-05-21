using FarmsToFeedUs.Data;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace FarmsToFeedUs.ImportService.Services
{
    public class ImportService : IImportService
    {
        public ImportService(IFarmRepository farmRepository, IFarmDataService farmDataService, IPostcodeService postcodeService, ILogger<ImportService> logger)
        {
            FarmRepository = farmRepository;
            FarmDataService = farmDataService;
            PostcodeService = postcodeService;
            Logger = logger;
        }

        private IFarmRepository FarmRepository { get; }
        private IFarmDataService FarmDataService { get; }
        private IPostcodeService PostcodeService { get; }
        private ILogger Logger { get; }

        public async Task BeginAsync()
        {
            var importedFarms = await FarmDataService.GetFarmDataAsync();
            var dbFarms = await FarmRepository.ListAllAsync();

            Logger.LogInformation($"Found {importedFarms.Count} farms in data service and {dbFarms.Count} farms in the database");

            foreach (var importedFarm in importedFarms)
            {
                var dbFarm = dbFarms.FirstOrDefault(f => f.Name == importedFarm.Name);

                if (dbFarm == null)
                    await CreateFarmAsync(importedFarm);
                else
                    await UpdateFarmAsync(importedFarm, dbFarm);
            }

            foreach (var dbFarm in dbFarms.Where(f => !importedFarms.Any(i => i.Name == f.Name)))
            {
                await DeleteFarmAsync(dbFarm);
            }
        }

        private async Task CreateFarmAsync(FarmData farmData)
        {
            // Require a minimum of a name
            if (string.IsNullOrWhiteSpace(farmData.Name))
                return;

            Logger.LogInformation($"Creating farm \"{farmData.Name}\"");

            var dbFarm = await MakeFarmFromFarmDataAsync(farmData);
            await FarmRepository.CreateAsync(dbFarm);
        }

        private async Task UpdateFarmAsync(FarmData farmData, Farm dbFarm)
        {
            if (IsDifferent(farmData, dbFarm))
            {
                // Require a minimum of a name
                if (string.IsNullOrWhiteSpace(farmData.Name))
                    return;

                Logger.LogInformation($"Updating farm \"{farmData.Name}\"");

                var updatedFarm = await MakeFarmFromFarmDataAsync(farmData);

                // Mark it as an update of the existing db record
                updatedFarm.VersionNumber = dbFarm.VersionNumber;

                await FarmRepository.UpdateAsync(updatedFarm);
            }
        }

        private async Task DeleteFarmAsync(Farm dbFarm)
        {
            Logger.LogInformation($"Deleting farm \"{dbFarm.Name}\"");

            await FarmRepository.DeleteAsync(dbFarm);
        }

        private bool IsDifferent(FarmData farmData, Farm dbFarm)
        {
            return farmData.Name != dbFarm.Name ||
                   farmData.Town != dbFarm.Town ||
                   farmData.County != dbFarm.County ||
                   farmData.Postcode != dbFarm.Postcode;
        }

        private async Task<Farm> MakeFarmFromFarmDataAsync(FarmData importedFarm)
        {
            PostcodeResult? postcodeLookup = null;

            if (!string.IsNullOrWhiteSpace(importedFarm.Postcode))
                postcodeLookup = await PostcodeService.GetPostcodeAsync(importedFarm.Postcode);

            if (postcodeLookup == null && !string.IsNullOrWhiteSpace(importedFarm.Town))
                postcodeLookup = await PostcodeService.GetPlaceAsync(importedFarm.Town);

            if (postcodeLookup == null && !string.IsNullOrWhiteSpace(importedFarm.County))
                postcodeLookup = await PostcodeService.GetPlaceAsync(importedFarm.County);

            return new Farm
            {
                Name = importedFarm.Name ?? "-- missing --",
                Town = importedFarm.Town,
                County = importedFarm.County,
                Postcode = importedFarm.Postcode,
                Latitude = postcodeLookup?.Latitude,
                Longitude = postcodeLookup?.Longitude
            };
        }

    }
}
