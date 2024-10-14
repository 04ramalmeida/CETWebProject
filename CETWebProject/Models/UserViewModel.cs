using System;
using System.ComponentModel.DataAnnotations;

namespace CETWebProject.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Full name")]
        public string Name { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public string Role { get; set; }

        public DateTime SignUpDateTime { get; set; }

        [Display(Name = "Profile Picture")]
        public string ProfileFullPath { get; set; }
    }
}
