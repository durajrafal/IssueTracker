using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.ValueObjects;

namespace IssueTracker.Application.Common.Helpers
{
    public static class AuditEventsExtensions
    {
        public static async Task SeedWithUsersAsync(this ICollection<AuditEvent> auditEvents, IUserService userService)
        {
            foreach (var auditEvent in auditEvents)
            {
                auditEvent.ModifiedBy = await userService.GetUserByIdAsync(auditEvent.ModifiedById);
            }
        }
    }
}
