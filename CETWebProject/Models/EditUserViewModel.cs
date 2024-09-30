using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace CETWebProject.Models
{
    public class EditUserViewModel
    {
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        public string Username { get; set; }

        [Required]
        [MaxLength (100, ErrorMessage = "This field can only contain {1} characters.")]
        public string Address { get; set; }

        [Required]
        [MaxLength(9, ErrorMessage = "The phone number has to contain at least 9 characters")]
        public string PhoneNumber { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}
