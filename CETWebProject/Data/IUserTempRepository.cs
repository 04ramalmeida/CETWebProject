using CETWebProject.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CETWebProject.Data
{
    public interface IUserTempRepository : IGenericRepository<UserTemp>
    {
        Task<ICollection<UserTemp>> GetAllRequestsAsync();

        
    }
}
