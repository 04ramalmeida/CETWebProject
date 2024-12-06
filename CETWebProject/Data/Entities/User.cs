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

        public Guid ProfilePicId { get; set; }

        public string ProfilePicFullPath => ProfilePicId == Guid.Empty 
            ? "http://aguasjalm.somee.com/img/avatar.jpg"
            : $"https://jalmaquablob.blob.core.windows.net/avatars/{ProfilePicId}";

    }
}
