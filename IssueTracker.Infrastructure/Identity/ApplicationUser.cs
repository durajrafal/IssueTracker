using IssueTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {   
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ApplicationUser(string email, string firstName, string lastName)
            :base(email)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
