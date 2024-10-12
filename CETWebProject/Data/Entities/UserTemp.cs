using CETWebProject.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CETWebProject.Data.Entities
{
    public class UserTemp : IEntity
    {
        public int Id { get; set; } 

        public string FirstName { get; set; }


        public string LastName { get; set; }


        public string Username { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Role { get; set; }
    }
}
