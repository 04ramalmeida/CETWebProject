using System.Collections.Generic;
using CETWebProject.Data.Entities;
using System.Linq;
using System.Threading.Tasks;
using CETWebProject.Models;

namespace CETWebProject.Data
{
    public interface IWaterMeterRepository : IGenericRepository<WaterMeter>
    {
        Task<ICollection<WaterMeter>> GetWaterMetersByUserAsync(string userName);

        Task AddWaterMeterAsync(string userName);

        Task DeleteWaterMeterAsync(int meterId);

        Task AddReadingAsync(int meterId, AddReadingViewModel model);

        Task<WaterMeter> GetWaterMeterWithReadings(int id);
        
        Task<Reading> GetReadingByIdAsync(int id);
        
        Task UpdateReading (Reading reading);
        
        int GetMeterIdByReading (Reading reading);
        
        Task DeleteReadingAsync (Reading reading);

        Task RequestMeter (RequestMeterViewModel model);
    }
}
