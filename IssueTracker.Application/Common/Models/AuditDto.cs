using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Common;
using IssueTracker.Domain.ValueObjects;

namespace IssueTracker.Application.Common.Models
{
    public class AuditDto
    {
        public DateTime Created { get; set; }
        public User CreatedByUser { get; set; }
        public DateTime? LastModified { get; set; }
        public User? LastModifiedBy { get; set; }
        public PaginatedList<AuditEvent> AuditEvents { get; set; }

        public static AuditDto Create(IAuditableEntity entity, IUserService userService)
        {
            return new AuditDto()
            {
                Created = entity.Created,
                CreatedByUser = userService.GetUserByIdAsync(entity.CreatedBy).GetAwaiter().GetResult()!,
                LastModified = entity.LastModified,
                LastModifiedBy = userService.GetUserByIdAsync(entity.LastModifiedById).GetAwaiter().GetResult()
            };
        }
    }
}
