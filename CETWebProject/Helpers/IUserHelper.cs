using CETWebProject.Data.Entities;
using CETWebProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CETWebProject.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetUserById(string id); // TODO: change to GetUserByIdAsync

        Task<IdentityResult> AddUserAsync(User user);

        Task CheckRoleAsync(string roleName);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task ChangeUserRolesAsync(User user, string roleName);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        ICollection<UserViewModel> GetAllUsers();

        ICollection<UserViewModel> GetAllCustomers();

        SelectList GetAllRoles();


        string GetUserRole(User user);

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task<SignInResult> ValidatePasswordAsync(User user, string password);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> AddPasswordAsync (User user,  string password);
    }
}
