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

        public Task CreateAsync(Farm farm)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(Farm updatedFarm)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(Farm dbFarm)
        {
            throw new System.NotImplementedException();
        }
    }
}
