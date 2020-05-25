using FarmsToFeedUs.Data;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace FarmsToFeedUs.ImportService.Services
{
    public class ImportService : IImportService
    {
        public ImportService(IFarmRepository farmRepository, IFarmDataService farmDataService, IFarmDataToFarmService farmDataToFarmService, ILogger<ImportService> logger)
        {
            FarmRepository = farmRepository;
            FarmDataService = farmDataService;
            FarmDataToFarmService = farmDataToFarmService;
            Logger = logger;
        }

        private IFarmRepository FarmRepository { get; }
        private IFarmDataService FarmDataService { get; }
        private IFarmDataToFarmService FarmDataToFarmService { get; }
        private ILogger Logger { get; }

        public async Task BeginAsync(bool forceUpdate = false)
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
                    await UpdateFarmAsync(importedFarm, dbFarm, forceUpdate);
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

            var dbFarm = await FarmDataToFarmService.MakeFarmFromFarmDataAsync(farmData);
            await FarmRepository.CreateAsync(dbFarm);
        }

        private async Task UpdateFarmAsync(FarmData farmData, Farm dbFarm, bool forceUpdate)
        {
            if (IsDifferent(farmData, dbFarm) || forceUpdate)
            {
                // Require a minimum of a name
                if (string.IsNullOrWhiteSpace(farmData.Name))
                    return;

                Logger.LogInformation($"Updating farm \"{farmData.Name}\"");

                var updatedFarm = await FarmDataToFarmService.MakeFarmFromFarmDataAsync(farmData);

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
    }
}
