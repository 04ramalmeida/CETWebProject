using CETWebProject.Data.Entities;
using CETWebProject.Helpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
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
        public async Task<IQueryable<WaterMeter>> GetWaterMetersByUserIdAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            return _context.waterMeters.Where(m => m.User == user);
        }
    }
}
