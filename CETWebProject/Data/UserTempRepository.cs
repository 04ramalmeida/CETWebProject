﻿using CETWebProject.Data.Entities;
using CETWebProject.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<UserTemp> FindByEmailAsync(string email)
        {
            return await _context.usersTemp
                .Where(u => u.Username == email)
                .FirstOrDefaultAsync();
        }

        
    }
}
