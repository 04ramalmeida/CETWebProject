using CETWebProject.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CETWebProject.Data
{
    public interface IAlertRepository : IGenericRepository<Alert>
    {
        Task<ICollection<Alert>> GetByUserAsync(string username);
    }
}
