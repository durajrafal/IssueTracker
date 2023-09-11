using IssueTracker.Domain.ValueObjects;

namespace IssueTracker.Domain.Common
{
    public interface IAuditableEntity
    {
        public int Id { get; set; } 
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string? LastModifiedBy { get; set; }
        public ICollection<AuditEvent> AuditEvents { get; set; }
    }
}
