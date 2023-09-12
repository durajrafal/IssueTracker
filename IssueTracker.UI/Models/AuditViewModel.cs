using IssueTracker.Application.Common.Models;
using IssueTracker.Domain.ValueObjects;

namespace IssueTracker.UI.Models
{
    public class AuditViewModel
    {
        public DateTime Created { get; set; }
        public User CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public User? LastModifiedBy { get; set; }
        public PaginatedList<AuditEvent> AuditEvents { get; set; }

        public static AuditViewModel Create(AuditDto auditDto)
        {
            return new AuditViewModel
            {
                Created = auditDto.Created,
                CreatedBy = auditDto.CreatedByUser,
                LastModified = auditDto.LastModified,
                LastModifiedBy = auditDto.LastModifiedBy,
                AuditEvents = auditDto.AuditEvents
            };
        }
    }
}
