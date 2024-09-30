using CETWebProject.Data;
using CETWebProject.Data.Entities;
using CETWebProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CETWebProject.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        public UserHelper
            (DataContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task ChangeUserRolesAsync(User user, string roleName)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles != null)
            {
                await _userManager.RemoveFromRolesAsync(user, userRoles);
            }
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }
        }

        public SelectList GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return new SelectList(roles);
        }

        public ICollection<UserViewModel> GetAllUsers()
        {
            var userList = _context.Users.ToList();
            ICollection<UserViewModel> users = userList.Cast<User>().Select(item => new UserViewModel
            {
                Id = item.Id,
                Name = item.FullName,
                Email = item.Email,
                Role = _userManager.GetRolesAsync(item).Result.FirstOrDefault(),
                SignUpDateTime = item.SignUpDateTime,
            }).ToList();
            return users;
        }

        public async Task<User> GetUserByEmailAsync(string userName)
        {
            return await _userManager.FindByEmailAsync(userName);
        }

        public async Task<User> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public string GetUserRole(User user)
        {
            return _userManager.GetRolesAsync(user).Result.FirstOrDefault();
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(
                model.Username,
                model.Password,
                model.RememberMe,
                false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }
    }
}
