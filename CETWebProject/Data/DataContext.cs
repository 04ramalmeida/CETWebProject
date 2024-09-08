

using CETWebProject.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CETWebProject.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Reading> MonthlyReadings { get; set; }
    }
}
