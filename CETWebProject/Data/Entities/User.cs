using Microsoft.AspNetCore.Identity;
using System;

namespace CETWebProject.Data.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public DateTime SignUpDateTime { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string ProfilePicUrl { get; set; }

        public string ProfilePicFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ProfilePicUrl))
                {
                    return "https://localhost:44321/img/avatar.jpg";
                }
                return $"https://localhost:44321{ProfilePicUrl.Substring(1)}";
            }
        }

    }
}
