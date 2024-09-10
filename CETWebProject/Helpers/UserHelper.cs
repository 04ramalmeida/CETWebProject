using CETWebProject.Data;
using CETWebProject.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CETWebProject.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        public UserHelper(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<User> GetUserByEmailAsync(string userName)
        {
            return await _userManager.FindByEmailAsync(userName);
        }
    }
}
