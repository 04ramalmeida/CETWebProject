using CETWebProject.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CETWebProject.Data
{
    public class EchelonRepository : GenericRepository<Echelons>, IEchelonRepository
    {
        private readonly DataContext _context;
        public EchelonRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ICollection<Echelons>> GetAll()
        {
            return await _context.echelons.ToListAsync();
            
        }

        public async Task UpdateEchelons(ICollection<Echelons> updatedEchelons)
        {
            foreach (var echelon in updatedEchelons)
            {
                var selectedEchelon = _context.echelons.Find(echelon.Id);
                selectedEchelon.Value = echelon.Value;
                
            }
            await _context.SaveChangesAsync();
        }
    }
}
