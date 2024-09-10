using CETWebProject.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace CETWebProject.Data
{
    public interface IWaterMeterRepository : IGenericRepository<WaterMeter>
    {
        Task<IQueryable<WaterMeter>> GetWaterMetersByUserIdAsync(string userName);
    }
}
