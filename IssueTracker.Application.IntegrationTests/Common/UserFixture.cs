using IssueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.IntegrationTests.Common
{
    public partial class UserFixture
    {
        public static string _currentUserId;

        public static string GetCurrentUserId()
        {
            return _currentUserId;
        }

        public static async Task<string> RunWithNoClaims()
        {
            return await RunAsUserAsync("no@claim.com", "password", Array.Empty<Claim>());
        }

        private static async Task<string> RunAsUserAsync(string userName, string password, Claim[] claims)
        {
            var factory = new CustomWebApplicationFactory();
            using (var scope = factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var user = userManager.FindByEmailAsync(userName).Result;

                if (user == null)
                {
                    user = new ApplicationUser { UserName = userName, Email = userName };

                    var result = await userManager.CreateAsync(user);

                    if (claims.Any())
                    {
                        await userManager.AddClaimsAsync(user, claims);
                    }

                    if (result.Succeeded)
                    {
                        _currentUserId = user.Id;
                        return _currentUserId;
                    }

                    throw new Exception($"Unable to create {userName}");
                }

                return user.Id;

            }
        }
    }
}
