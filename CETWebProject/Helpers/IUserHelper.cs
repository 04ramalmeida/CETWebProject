using CETWebProject.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CETWebProject.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);
    }
}
