using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmsToFeedUs.Data
{
    public class FarmDynamoDbRepository : IFarmRepository
    {
        public FarmDynamoDbRepository(IDynamoDBContext dynamoDBContext, IGeohashService geohashService)
        {
            DynamoDBContext = dynamoDBContext;
            GeohashService = geohashService;
        }

        private IDynamoDBContext DynamoDBContext { get; }
        public IGeohashService GeohashService { get; }

        public async Task<List<Farm>> ListAllAsync()
        {
            var asyncScan = DynamoDBContext.ScanAsync<Farm>(new List<ScanCondition>() { });

            return (await asyncScan.GetRemainingAsync()).ToList();
        }

        public async Task<List<Farm>> ListByLatLongAsync(double latitude, double longitude)
        {
            var ranges = GeohashService.GetRanges(latitude, longitude);
            var scanConditions = ranges.Select(r => new ScanCondition(nameof(Farm.GeoHash), ScanOperator.Between, r.Min, r.Max));

            var asyncScan = DynamoDBContext.ScanAsync<Farm>(scanConditions);

            return (await asyncScan.GetRemainingAsync()).ToList();
        }

        public async Task CreateAsync(Farm farm)
        {
            await DynamoDBContext.SaveAsync(farm);
        }

        public async Task UpdateAsync(Farm farm)
        {
            await DynamoDBContext.SaveAsync(farm);
        }

        public async Task DeleteAsync(Farm farm)
        {
            await DynamoDBContext.DeleteAsync(farm);
        }
    }
}
