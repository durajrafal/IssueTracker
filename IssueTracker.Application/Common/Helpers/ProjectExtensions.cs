using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Common.Helpers
{
    public static class ProjectExtensions
    {
        public static void AddNewOrExistingMember(this Project entity, DbSet<Member> existingMembers, string userId)
        {
            var member = existingMembers.FirstOrDefault(x => x.UserId == userId);
            if (member == null)
            {
                entity.Members.Add(new Member { UserId = userId });
            }
            else
            {
                entity.Members.Add(member);
            }
        }

        public static void PopulateMembersWithUsers(this Project entity, IUserService userService)
        {
            entity.Members.ToList().ForEach(async x => x.User = await userService.GetUserByIdAsync(x.UserId));
        }
    }
}
