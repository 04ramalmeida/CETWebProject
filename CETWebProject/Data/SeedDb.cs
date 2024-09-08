using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CETWebProject.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private Random _random;
        public SeedDb(DataContext context)
        {
            _context = context;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();
        }
    }
}
