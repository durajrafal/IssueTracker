using IssueTracker.Domain.Common;
using IssueTracker.Domain.Enums;

namespace IssueTracker.Domain.Entities
{
    public class Issue : AuditableEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public PriorityLevel Priority { get; set; } = PriorityLevel.None;
        public WorkingStatus Status { get; set; } = WorkingStatus.Pending;
        public ICollection<Member> Members { get; set; } = new List<Member>();
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
