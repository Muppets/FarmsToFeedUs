using FarmsToFeedUs.Data;
using System.Threading.Tasks;

namespace FarmsToFeedUs.ImportService.Services
{
    public interface IFarmDataToFarmService
    {
        Task<Farm> MakeFarmFromFarmDataAsync(FarmData farmData);
    }
}
