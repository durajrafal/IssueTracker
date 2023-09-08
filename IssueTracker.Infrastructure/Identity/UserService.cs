using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Constants;
using IssueTracker.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IssueTracker.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICurrentUserService _currentUserService;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _currentUserService = currentUserService;
        }   

        public IEnumerable<User> GetAllUsers()
        {
            var output = new List<User>();
            foreach (var user in _userManager.Users)
            {
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

        public async Task<User?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }
            return new User
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
        }

        public async Task<IEnumerable<Claim>?> GetUserClaimsAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return default;
            return await _userManager.GetClaimsAsync(user);
        }

        public async Task<Claim?> GetUserRoleClaimAsync(string id)
        {
            var claims = await GetUserClaimsAsync(id);
            return claims?.FirstOrDefault(x => x.Type == ClaimTypes.Role);
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

        public async Task AddProjectAccessClaimToUserAsync(string userId, int projectId)
        {
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser is not null)
            {
                await _userManager.AddClaimAsync(appUser, new Claim(AppClaimTypes.ProjectAccess, projectId.ToString()));
            }
        }

        public async Task RemoveProjectAccessClaimFromUserAsync(string userId, int projectId)
        {
            var appUser = await _userManager.FindByIdAsync(userId);
            if(appUser is not null)
            {
                var claims = await _userManager.GetClaimsAsync(appUser);
                var claim = claims.FirstOrDefault(x => x.Type == AppClaimTypes.ProjectAccess && x.Value == projectId.ToString());
                if (claim is not null)
                    await _userManager.RemoveClaimAsync(appUser, claim);
            }
        }

        public async Task RefreshCurrentUserSignInAsync()
        {
            var appUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
            if (appUser is not null)
                await _signInManager.RefreshSignInAsync(appUser);
        }

        public string GetCurrentUserId()
        {
            return _currentUserService.UserId;
        }
    }
}
