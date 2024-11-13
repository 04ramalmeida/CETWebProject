

using CETWebProject.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CETWebProject.Models;

namespace CETWebProject.Data
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Reading> monthlyReadings { get; set; }

        public DbSet<WaterMeter> waterMeters { get; set; }

        public DbSet<Invoice> invoices { get; set; }

        public DbSet<MeterTemp> metersTemp { get; set; }

        public DbSet<UserTemp> usersTemp { get; set; }

        public DbSet<Echelons> echelons { get; set; }

        public DbSet<Alert> alerts { get; set; }
    }
}
