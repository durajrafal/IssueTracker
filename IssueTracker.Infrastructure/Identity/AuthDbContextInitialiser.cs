using IssueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IssueTracker.Infrastructure
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
            catch (Exception)
            {
                throw;
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
                var user = new ApplicationUser(userName, claimRoleValue, "Tester") { 
                    EmailConfirmed = true,
                };
                await _userManager.CreateAsync(user, "Pass123");

                var claim = new Claim(ClaimTypes.Role, claimRoleValue);
                await _userManager.AddClaimAsync(user, claim);
            }
        }
    }
}
