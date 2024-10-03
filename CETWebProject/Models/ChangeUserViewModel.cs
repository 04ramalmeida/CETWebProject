using System.ComponentModel.DataAnnotations;
using System;

namespace CETWebProject.Models
{
    public class ChangeUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        [MaxLength(9, ErrorMessage = "A phone number can only be 9 characters long.")]
        public string PhoneNumber { get; set; }

    }
}
