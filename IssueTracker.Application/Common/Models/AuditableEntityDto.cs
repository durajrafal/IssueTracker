using IssueTracker.Domain.ValueObjects;

namespace IssueTracker.Application.Common.Models
{
    public class AuditableEntityDto
    {
        public DateTime Created { get; set; }
        public User CreatedByUser { get; set; }
        public DateTime? LastModified { get; set; }
        public User? LastModifiedBy { get; set; }
        public PaginatedList<AuditEvent> AuditEvents { get; set; }
    }
}
