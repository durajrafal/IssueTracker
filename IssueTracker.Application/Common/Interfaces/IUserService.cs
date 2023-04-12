using IssueTracker.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(string id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IList<Claim>> GetUserClaimsAsync(string id);
        Task ChangeUserRoleClaimAsync(string id, string newRoleClaimValue);
        
    }
}
