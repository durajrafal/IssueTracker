using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Common.Helpers
{
    public static class MembersExtensions
    {
        public static void AddNewOrExistingMember(this ICollection<Member> members, DbSet<Member> existingMembers, string userId)
        {
            var member = existingMembers.FirstOrDefault(x => x.UserId == userId);
            if (member == null)
            {
                members.Add(new Member { UserId = userId });
            }
            else
            {
                members.Add(member);
            }
        }

        public static IEnumerable<Member> SyncExistingMembersId(this IEnumerable<Member> members, DbSet<Member> existingMembers)
        {
            members.Where(x => x.Id == 0).ToList()
                .ForEach(x => {
                    var existingMember = existingMembers.AsNoTracking().FirstOrDefault(y => y.UserId == x.UserId);
                    if (existingMember is not null)
                    {
                        x.Id = existingMember.Id;
                    }
                });
            return members;
        }

        public static async Task<ICollection<Member>> SyncMembersWithUsers(this ICollection<Member> members, IUserService userService)
        {
            await PopulateMembersWithUsersAsync(members, userService);
            RemoveMembersWithoutExistingUsers(members);

            return members;
        }

        private static async Task PopulateMembersWithUsersAsync(this ICollection<Member> members, IUserService userService)
        {
            foreach (var member in members)
            {
                member.User = await userService.GetUserByIdAsync(member.UserId);
            }
        }

        private static void RemoveMembersWithoutExistingUsers(this ICollection<Member> members)
        {
            var emptyMembers = members.Where(x => x.User == null).ToList();
            emptyMembers.ForEach(x => members.Remove(x));
        }
    }
}
