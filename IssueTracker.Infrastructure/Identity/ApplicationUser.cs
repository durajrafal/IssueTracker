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
        public ApplicationUser(string userName)
            :base(userName)
        {
            
        }
    }
}
