using CETWebProject.Data.Entities;
using CETWebProject.Helpers;
using CETWebProject.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
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
                UsageAmount = model.UsageAmount,
            });
            await _context.SaveChangesAsync();
        }

        public async Task AddWaterMeterAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            var newMeter = new WaterMeter
            {
                Username = userName,
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
            return _context.waterMeters.Where(m => m.Username == user.UserName)
                .OrderBy(m => m.Id).ToList();
        }

        public async Task<WaterMeter> GetWaterMeterWithReadings(int id)
        {
            return await _context.waterMeters.Where(m => m.Id == id)
                .Include(m => m.Readings)
                .FirstOrDefaultAsync();
        }

        public async Task<WaterMeter> GetWaterMeterWithUser(int id)
        {
            return await _context.waterMeters.Where(m => m.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Reading> GetReadingByIdAsync(int id)
        {
            return await _context.monthlyReadings.Where(m => m.Id == id)
                .Include(m => m.WaterMeter)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateReading(Reading reading)
        {
            var meter = await _context.waterMeters.FindAsync(GetMeterIdByReading(reading));
            if (meter == null)
            {
                return;
            }

            _context.monthlyReadings.Update(reading);
            await _context.SaveChangesAsync();
        }

        public int GetMeterIdByReading(Reading reading)
        {
            return _context.monthlyReadings
                .Where(r => r.Id == reading.Id)
                .Select(r => r.WaterMeter)
                .FirstOrDefault().Id;
        }

        public async Task DeleteReadingAsync(Reading reading)
        {
            
            _context.Set<Reading>().Remove(reading);
            await SaveAllAsync();
        }

        public async Task RequestMeter(RequestMeterViewModel model)
        {
            
            _context.metersTemp.Add(new MeterTemp
            {
                Username = model.Username,
                Date = model.Date,
            });
            await SaveAllAsync();
        }

        /*public async Task<ICollection<MeterTemp>> GetRequestsByUser(string id)
        {
            return await _context.metersTemp
                .Where(m => m.User.Id == id)
                .ToListAsync();
        }*/

        public Task<MeterTemp> GetRequestById(int id)
        {
            return _context.metersTemp
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task RemoveRequest(MeterTemp request)
        {
            _context.Remove(request);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<MeterTemp>> GetAllMeterTemp()
        {
            return await _context.metersTemp.ToListAsync();
        }

        public async Task ApproveMeterRequest(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                var userTemp = await _context.usersTemp
               .Where(u => u.Username == userName)
               .FirstOrDefaultAsync();

                if (userTemp != null)
                {
                    userTemp.IsMeterApproved = true;
                    await _context.SaveChangesAsync();
                } 
            }
            else
            {
                await AddWaterMeterAsync(userName);
            }
           
        }
    }
}
