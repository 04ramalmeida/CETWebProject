﻿using CETWebProject.Data;
using CETWebProject.Data.Entities;
using CETWebProject.Helpers;
using CETWebProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        private readonly IBlobHelper _blobHelper;
        private readonly IConfiguration _configuration;
        private readonly IUserTempRepository _userTempRepository;
        private readonly IWaterMeterRepository _waterMeterRepository;

        public AccountController(IUserHelper userHelper,
            IMailHelper mailHelper,
            IBlobHelper blobHelper,
            IConfiguration configuration,
            IUserTempRepository userTempRepository,
            IWaterMeterRepository waterMeterRepository)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _blobHelper = blobHelper;
            _configuration = configuration;
            _userTempRepository = userTempRepository;
            _waterMeterRepository = waterMeterRepository;
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
            if (user == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }
            UserViewModel model = new UserViewModel
            {
                Id = user.Id,
                Name = user.FullName,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Role = _userHelper.GetUserRole(user),
                SignUpDateTime = user.SignUpDateTime,
                ProfileFullPath = user.ProfilePicFullPath,
            };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userHelper.GetUserById(id);
            if (user == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }
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
            } else
            {
                return new NotFoundViewResult("UserNotFound");
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
                model.ProfilePictureID = user.ProfilePicId;
                model.ProfileFullPath = user.ProfilePicFullPath;
            } else
            {
                return new NotFoundViewResult("UserNotFound");
            }

            return View(model);
       }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid profileId = Guid.Empty;
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        profileId = await _blobHelper.UploadBlobAsync(model.ImageFile, "avatars");
                    }
                    var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    if (user != null)
                    {
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        user.Address = model.Address;
                        user.PhoneNumber = model.PhoneNumber;
                        user.ProfilePicId = profileId;
                        model.ProfileFullPath = user.ProfilePicFullPath;

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
                    else
                    {
                        return new NotFoundViewResult("UserNotFound");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _userHelper.GetUserById(model.Id) == null)
                    {
                        return new NotFoundViewResult("UserNotFound");
                    } 
                    else
                    {
                        throw;
                    }
                }
            }
            /**/
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
                else
                {
                    return new NotFoundViewResult("UserNotFound");
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
                return new NotFoundViewResult("UserNotFound");
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
                if (user == null) 
                {
                    return new NotFoundViewResult("UserNotFound");
                }

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

        [Authorize(Roles = "Employee")]
        public IActionResult EmployeeCenterIndex()
        {
            var model = _userHelper.GetAllCustomers();
            return View(model);
        }

        public IActionResult RequestAccount()
        {
            var model = new AddNewUserViewModel
            {
                Role = "Customer"
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RequestAccount(AddNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _userTempRepository.CreateAsync(new UserTemp
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Username = model.Username,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    Role = model.Role
                });
                ViewBag.Message = "Please await for your request to be accepted.";
                await _waterMeterRepository.RequestMeter(new RequestMeterViewModel
                {
                    Username = model.Username,
                    Date = DateTime.Now
                });
            }
            return View();
        }

        [Authorize(Roles =  "Admin")]
        public async Task<IActionResult> AdminUserRequestsAsync()
        {
            var model = await _userTempRepository.GetAllRequestsAsync();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AcceptUserRequest(int id)
        {
            var request = await _userTempRepository.GetByIdAsync(id);
            request.Username = request.Username.Replace("\n","");
            if (request == null)
            {
                return new NotFoundViewResult("UserRequestNotFound");
            }
            var user = await _userHelper.GetUserByEmailAsync(request.Username);
            if (user == null)
            {
                user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.Username,
                    Email = request.Username,
                    Address = request.Address,
                    PhoneNumber = request.PhoneNumber,
                    SignUpDateTime = DateTime.Now
                };
            }
            var result = await _userHelper.AddUserAsync(user);

            

            if (result != IdentityResult.Success)
            {
                ModelState.AddModelError(string.Empty, "The user could not be created");
                return View(request);
            }

            await _waterMeterRepository.AddWaterMeterAsync(request.Username);

            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            string tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = user.Id,
                token = myToken
            }, protocol: HttpContext.Request.Scheme);

            Response response = _mailHelper.SendEmail(request.Username,
                "Email confirmation",
                "To finish your registration, please click on the following link." +
                "</br>" +
                $"<a href=\"{tokenLink}\">Confirm Email</a>");


            await _userHelper.ChangeUserRolesAsync(user, request.Role);

            if (response.IsSuccess)
            {
                ViewBag.Message = "The instructions have been sent to the users.";
                await _userTempRepository.DeleteAsync(request);
                return View(request);
            }

            ModelState.AddModelError(string.Empty, "The user couldn't be registered.");

            return View(request);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserRequestDetailsAsync(int id)
        {
            var request = await _userTempRepository.GetByIdAsync(id);
            if (request == null)
            {
                return new NotFoundViewResult("UserRequestNotFound");
            }
            return View(request);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DenyUserRequestAsync(int id) 
        {
            var request = await _userTempRepository.GetByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            await _userTempRepository.DeleteAsync(request);
            return RedirectToAction("AdminUserRequests");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}
