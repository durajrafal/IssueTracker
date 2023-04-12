using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Common.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
                    await _userManager.AddClaimAsync(user,newRoleClaim);
                else
                    await _userManager.ReplaceClaimAsync(user, previousRoleClaim, newRoleClaim);
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var output = new List<UserDto>();
            foreach (var user in _userManager.Users)
            {
                var claims = await GetUserClaimsAsync(user.Id);
                output.Add(new UserDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RoleClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value,
                });
            }

            return output;
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var claims = await GetUserClaimsAsync(id);
            return new UserDto
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value,
            };
        }

        public async Task<IList<Claim>> GetUserClaimsAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return await _userManager.GetClaimsAsync(user);
        }
    }
}
