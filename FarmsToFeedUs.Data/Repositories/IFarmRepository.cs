using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmsToFeedUs.Data
{
    public interface IFarmRepository
    {
        Task<List<Farm>> ListAllAsync();
        Task<List<Farm>> ListByLatLongAsync(double latitude, double longitude);
        Task DeleteAsync(Farm farm);
        Task UpdateAsync(Farm farm);
        Task CreateAsync(Farm farm);
    }
}
