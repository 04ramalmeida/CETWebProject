using CETWebProject.Data.Entities;
using CETWebProject.Helpers;
using CETWebProject.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CETWebProject.Data
{
    public class WaterMeterRepository : GenericRepository<WaterMeter>, IWaterMeterRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public WaterMeterRepository(DataContext context, IUserHelper userHelper) : base(context) 
        {
            _context = context;
            _userHelper = userHelper;
        }


        public async Task AddReadingAsync(int meterId, AddReadingViewModel model)
        {
            var meter = await _context.waterMeters.FindAsync(meterId);
            if (meter == null)
            {
                return;
            }
            meter.Readings.Add(new Reading
            {
                ReadingTime = DateTime.Now,
                usageAmount = model.UsageAmount,
            });
            await _context.SaveChangesAsync();
        }

        public async Task AddWaterMeterAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            var newMeter = new WaterMeter
            {
                User = user
            };
            _context.Add(newMeter);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteWaterMeterAsync(int id)
        {
            var meter = await GetByIdAsync(id);
            await DeleteAsync(meter);
        }

        public async Task<ICollection<Reading>> GetReadingByMeterIdAsync(int meterId)
        {
            var meter = await GetByIdAsync(meterId);
            var result = await _context.waterMeters.Where(m => m.Id == meterId)
                .Select(m => m.Readings)
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task<ICollection<WaterMeter>> GetWaterMetersByUserAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            return _context.waterMeters.Where(m => m.User == user)
                .OrderBy(m => m.Id).ToList();
        }

        public async Task<WaterMeter> GetWaterMeterWithCities(int id)
        {
            return await _context.waterMeters.Where(m => m.Id == id)
                .Include(m => m.Readings)
                .FirstOrDefaultAsync();
        }
    }
}
