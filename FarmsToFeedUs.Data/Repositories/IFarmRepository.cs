using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmsToFeedUs.Data
{
    public interface IFarmRepository
    {
        Task<List<Farm>> ListAllAsync();
        Task DeleteAsync(Farm dbFarm);
        Task UpdateAsync(Farm updatedFarm);
        Task CreateAsync(Farm farm);
    }
}
