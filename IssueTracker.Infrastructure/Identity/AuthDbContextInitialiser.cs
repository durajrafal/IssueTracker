using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Infrastructure.Identity
{
    public class AuthDbContextInitialiser
    {
        private readonly AuthDbContext _ctx;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthDbContextInitialiser(AuthDbContext ctx, UserManager<ApplicationUser> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                if (_ctx.Database.IsSqlServer())
                {
                    await _ctx.Database.MigrateAsync();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await SeedWithDemoUsers();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task SeedWithDemoUsers()
        {
            await TrySeedUserAsync("dev@test.com", "Developer");
            await TrySeedUserAsync("manager@test.com", "Manager");
            await TrySeedUserAsync("admin@test.com", "Admin");
        }

        private async Task TrySeedUserAsync(string userName, string claimRoleValue)
        {
            if (!_userManager.Users.Any(x => x.UserName == userName))
            {
                var user = new ApplicationUser(userName);
                _userManager.CreateAsync(user, "Pass123").GetAwaiter().GetResult();

                var claim = new Claim(ClaimTypes.Role, claimRoleValue);
                await _userManager.AddClaimAsync(user, claim);
            }
        }
    }
}
