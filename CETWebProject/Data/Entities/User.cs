using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace CETWebProject.Data.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public DateTime SignUpDateTime { get; set; }

    }
}
