using CETWebProject.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CETWebProject.Data
{
    public interface IEchelonRepository : IGenericRepository<Echelons>
    {
        Task<ICollection<Echelons>> GetAll();

        Task UpdateEchelons(ICollection<Echelons> updatedEchelons);
    }

    
}
