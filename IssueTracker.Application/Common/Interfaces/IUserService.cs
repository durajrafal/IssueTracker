using IssueTracker.Domain.Models;
using System.Security.Claims;

namespace IssueTracker.Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(string id);
        Task<IEnumerable<Claim>> GetUserClaimsAsync(string id);
        Task<Claim> GetUserRoleClaimAsync(string id);
        Task ChangeUserRoleClaimAsync(string id, string newRoleClaimValue);
        
    }
}
