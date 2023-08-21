using IssueTracker.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.IntegrationTests.Library.Helpers
{
    public static class IdentityHelpers
    {
        public static async Task AddIdentityUserFromUserIdAsync(string userId, DatabaseHelpers database)
        {
            var appUser = new ApplicationUser(userId.Substring(0, 8), "Name", "Surname");
            appUser.Id = userId;
            await database.ActionAsync<AuthDbContext>(ctx => ctx.Users.AddAsync(appUser));
        }
    }
}
