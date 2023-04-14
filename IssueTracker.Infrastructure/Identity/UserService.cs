using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IssueTracker.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }   

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var output = new List<User>();
            foreach (var user in _userManager.Users)
            {
                var claims = await GetUserClaimsAsync(user.Id);
                output.Add(new User
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                });
            }
            return output;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var claims = await GetUserClaimsAsync(id);
            return new User
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
        }

        public async Task<IEnumerable<Claim>> GetUserClaimsAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return await _userManager.GetClaimsAsync(user);
        }

        public async Task<Claim> GetUserRoleClaimAsync(string id)
        {
            var claims = await GetUserClaimsAsync(id);
            return claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
        }

        public async Task ChangeUserRoleClaimAsync(string id, string newRoleClaimValue)
        {
            var acceptableRoles = new List<string> { "Developer", "Manager", "Admin" };
            if (acceptableRoles.Contains(newRoleClaimValue))
            {
                var user = await _userManager.FindByIdAsync(id);
                var claims = await _userManager.GetClaimsAsync(user);
                var previousRoleClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
                var newRoleClaim = new Claim(ClaimTypes.Role, newRoleClaimValue);
                if (previousRoleClaim == null)
                    await _userManager.AddClaimAsync(user, newRoleClaim);
                else
                    await _userManager.ReplaceClaimAsync(user, previousRoleClaim, newRoleClaim);
            }
        }
    }
}
