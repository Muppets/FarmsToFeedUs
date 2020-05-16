using FarmsToFeedUs.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FarmsToFeedUs.ImportService.Services
{
    public class ImportService : IImportService
    {
        public ImportService(IFarmRepository farmRepository, IFarmDataService farmDataService, IPostcodeService postcodeService)
        {
            FarmRepository = farmRepository;
            FarmDataService = farmDataService;
            PostcodeService = postcodeService;
        }

        private IFarmRepository FarmRepository { get; }
        private IFarmDataService FarmDataService { get; }
        private IPostcodeService PostcodeService { get; }

        public async Task BeginAsync()
        {
            var importedFarms = await FarmDataService.GetFarmDataAsync();
            var dbFarms = await FarmRepository.ListAllAsync();

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

        private async Task CreateFarmAsync(FarmData importedFarm)
        {
            var dbFarm = await MakeFarmFromFarmDataAsync(importedFarm);
            await FarmRepository.CreateAsync(dbFarm);
        }

        private async Task UpdateFarmAsync(FarmData importedFarm, Farm dbFarm)
        {
            if (IsDifferent(importedFarm, dbFarm))
            {
                var updatedFarm = await MakeFarmFromFarmDataAsync(importedFarm);

                await FarmRepository.UpdateAsync(updatedFarm);
            }
        }

        private async Task DeleteFarmAsync(Farm dbFarm)
        {
            await FarmRepository.DeleteAsync(dbFarm);
        }

        private bool IsDifferent(FarmData importedFarm, Farm dbFarm)
        {
            return importedFarm.Name == dbFarm.Name &&
                   importedFarm.Town == dbFarm.Town &&
                   importedFarm.County == dbFarm.County &&
                   importedFarm.Postcode == dbFarm.Postcode;
        }

        private async Task<Farm> MakeFarmFromFarmDataAsync(FarmData importedFarm)
        {
            PostcodeResult? postcodeLookup = null;

            if (!string.IsNullOrWhiteSpace(importedFarm?.Postcode))
                postcodeLookup = await PostcodeService.GetPostcodeAsync(importedFarm.Postcode);

            return new Farm
            {
                Name = importedFarm!.Name ?? "-- missing -- ",
                Town = importedFarm.Town ?? "",
                County = importedFarm.County ?? "",
                Postcode = importedFarm.Postcode ?? "",
                Latitude = postcodeLookup?.Latitude,
                Longitude = postcodeLookup?.Longitude
            };
        }

    }
}
