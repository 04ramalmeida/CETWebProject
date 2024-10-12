using CETWebProject.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CETWebProject.Data
{
    public class UserTempRepository : GenericRepository<UserTemp>, IUserTempRepository
    {
        private readonly DataContext _context;

        public UserTempRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ICollection<UserTemp>> GetAllRequestsAsync()
        {
            return await _context.usersTemp
                .ToListAsync();
        }
    }
}
