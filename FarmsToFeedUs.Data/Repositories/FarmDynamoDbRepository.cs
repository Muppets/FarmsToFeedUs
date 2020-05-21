using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmsToFeedUs.Data
{
    public class FarmDynamoDbRepository : IFarmRepository
    {
        public FarmDynamoDbRepository(IDynamoDBContext dynamoDBContext)
        {
            DynamoDBContext = dynamoDBContext;
        }

        private IDynamoDBContext DynamoDBContext { get; }

        public async Task<List<Farm>> ListAllAsync()
        {
            var asyncScan = DynamoDBContext.ScanAsync<Farm>(new List<ScanCondition>() { });

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
