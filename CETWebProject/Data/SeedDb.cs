﻿using CETWebProject.Helpers;
using CETWebProject.Data.Entities;
using CETWebProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace CETWebProject.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private Random _random;
        private readonly IUserHelper _userHelper;
        private readonly IWaterMeterRepository _waterMeterRepository;
        public SeedDb(DataContext context, IUserHelper userHelper, IWaterMeterRepository waterMeterRepository)
        {
            _context = context;
            _random = new Random();
            _userHelper = userHelper;
            _waterMeterRepository = waterMeterRepository;
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            if (!_context.echelons.Any())
            {
                PopulateEchelons();
            }

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Employee");
            await _userHelper.CheckRoleAsync("Customer");

            var user = await _userHelper.GetUserByEmailAsync("email@email.com");

            if (user == null) 
            {
                user = new User
                {
                    FirstName = "Teste",
                    LastName = "Etset",
                    Email = "email@email.com",
                    UserName = "email@email.com",
                    PhoneNumber = "1234567890",
                    Address = "Morada 1234-123",
                    SignUpDateTime = DateTime.Now,
                };
                var result = await _userHelper.AddUserAsync(user);
                await _userHelper.AddPasswordAsync(user, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Couldn't create the user in seeder");
                }
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");

            

            if (!isInRole)
            {
                await _userHelper.ChangeUserRolesAsync(user, "Admin");
                

            }
            var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            await _userHelper.ConfirmEmailAsync(user, token);

            
            
        }

       


        private void PopulateEchelons()
        {
            double[] Values = { 0.3, 0.8, 1.2, 1.6 };

            for (int i = 0; i < 4; i++)
            {
                _context.echelons.Add(new Echelons
                {
                    Value = Values[i]
                });
            }
        }
    }
}
