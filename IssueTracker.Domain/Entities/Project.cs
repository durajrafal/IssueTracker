using IssueTracker.Domain.Common;
using IssueTracker.Domain.ValueObjects;

namespace IssueTracker.Domain.Entities
{
    public class Project : IAuditableEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Member> Members { get; init; } = new List<Member>();
        public ICollection<Issue> Issues { get; init; } = new List<Issue>();
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string? LastModifiedBy { get; set; }
        public ICollection<AuditEvent> AuditEvents { get; set; } = new List<AuditEvent>();
    }
}
