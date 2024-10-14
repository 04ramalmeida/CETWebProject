using CETWebProject.Data;
using CETWebProject.Data.Entities;
using CETWebProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IdentityResult> AddPasswordAsync(User user, string password)
        {
            return await _userManager.AddPasswordAsync(user, password);
        }

        public async Task<IdentityResult> AddUserAsync(User user)
        {
            return await _userManager.CreateAsync(user);
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

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public ICollection<UserViewModel> GetAllCustomers()
        {
            var userList = _context.Users.ToList();
            ICollection<UserViewModel> users = userList.Cast<User>()
                .Where(u => _userManager.IsInRoleAsync(u, "Customer").Result)
                .Select(item => new UserViewModel
            {
                Id = item.Id,
                Name = item.FullName,
                Email = item.Email,
                Role = _userManager.GetRolesAsync(item).Result.FirstOrDefault(),
                SignUpDateTime = item.SignUpDateTime,
            }).ToList();
            return users;
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
                ProfileFullPath = item.ProfilePicFullPath
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

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, false);
        }

        
    }
}
