using CETWebProject.Data;
using CETWebProject.Data.Entities;
using CETWebProject.Helpers;
using CETWebProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CETWebProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IConfiguration _configuration;
        private readonly IUserTempRepository _userTempRepository;

        public AccountController(IUserHelper userHelper,
            IMailHelper mailHelper,
            IConfiguration configuration,
            IUserTempRepository userTempRepository)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
            _userTempRepository = userTempRepository;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid) 
            { 
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded) 
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }
                    return this.RedirectToAction("Index", "Home");
                }
            }

            this.ModelState.AddModelError(string.Empty, "Failed to login.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult UserManagerIndex()
        {
            var model = _userHelper.GetAllUsers();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult UserManagerAddUser()
        {
            var model = new AddNewUserViewModel();
            model.Roles = _userHelper.GetAllRoles();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserManagerAddUser(AddNewUserViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Username);
            if (user == null)
            {
                user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Username,
                    Email = model.Username,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    SignUpDateTime = DateTime.Now
                };
            }
            var result = await _userHelper.AddUserAsync(user);

            if (result != IdentityResult.Success)
            {
                ModelState.AddModelError(string.Empty, "The user could not be created");
                return View(model);
            }

            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            string tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = user.Id,
                token = myToken
            }, protocol: HttpContext.Request.Scheme);

            Response response = _mailHelper.SendEmail(model.Username,
                "Email confirmation",
                "To finish your registration, please click on the following link." +
                "</br>" +
                $"<a href=\"{tokenLink}\">Confirm Email</a>");


            await _userHelper.ChangeUserRolesAsync(user, model.Role);

            if (response.IsSuccess)
            {
                ViewBag.Message = "The instructions have been sent to the users.";
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "The user couldn't be registered.");

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userHelper.GetUserById(id);
            UserViewModel model = new UserViewModel
            {
                Id = user.Id,
                Name = user.FullName,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Role = _userHelper.GetUserRole(user),
                SignUpDateTime = user.SignUpDateTime
            };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userHelper.GetUserById(id);
            var model = new EditUserViewModel();
            if (user != null) 
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Username = user.UserName;
                model.Address = user.Address; 
                model.PhoneNumber = user.PhoneNumber;
                model.Role = _userHelper.GetUserRole(user);
                model.Roles = _userHelper.GetAllRoles();
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Username);
            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                await _userHelper.ChangeUserRolesAsync(user, model.Role);

                var response = await _userHelper.UpdateUserAsync(user);


                if (response.Succeeded)
                {
                    ViewBag.Message = "User Updated!";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                }
            }
            return View(model);
        }

       public async Task<IActionResult> ChangeUser()
       {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            var model = new ChangeUserViewModel();
            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Address = user.Address;
                model.PhoneNumber = user.PhoneNumber;
            }

            return View(model);
       }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;

                var response = await _userHelper.UpdateUserAsync(user);
                if (response.Succeeded)
                {
                    ViewBag.Message = "Info updated.";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                }
                
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null) 
                {
                    var result = await _userHelper.ValidatePasswordAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:Key"]));

                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15),
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return this.Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ConfirmEmailViewModel
            {
                token = token,
            }; 

            return View();

            /*var result = await _userHelper.ConfirmEmailAsync(user, token);

            if (result.Succeeded) 
            { 
            }*/

        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            if (ModelState.IsValid) 
            {
                var user = await _userHelper.GetUserById(model.userId);

                var response = await _userHelper.ConfirmEmailAsync(user, model.token);

                if (response.Succeeded)
                {
                    await _userHelper.AddPasswordAsync(user, model.Password);
                    ViewBag.Message = "The password has been changed. You may sign in now.";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error");
            }

            return View(model);
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspond to a registered user.");
                    return View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                var link = this.Url.Action("ResetPassword", "Account", new {token = myToken}, protocol:HttpContext.Request.Scheme);

                Response response = _mailHelper.SendEmail(model.Email, "Water Company Password Reset", $"<h1>Password Reset</h1>" +
                    $"As you have requested to reset your password, click on this link:" +
                    $"</br> <a href=\"{link}\">Reset Password</a>");

                if (response.IsSuccess)
                {
                    ViewBag.Message = "The instructions to recover your password have been sent to your email.";
                }

                return View();
            }
            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.UserName);
            if (user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Password reset succesful";
                    return View();
                }
                ViewBag.Message = "Error while resetting the password.";
                return View(model);
            }

            ViewBag.Message = "User not found.";
            return View(model);
        }
    }
}
