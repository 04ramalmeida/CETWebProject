using System.ComponentModel.DataAnnotations;

namespace CETWebProject.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
