using CETWebProject.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CETWebProject.Data
{
    public class AlertRepository : GenericRepository<Alert>, IAlertRepository
    {
        private readonly DataContext _context;
        public AlertRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ICollection<Alert>> GetByUserAsync(string username) 
        {
            return await _context.alerts
                .Where(a => a.User.UserName == username)
                .ToListAsync();
        }
    }
}
