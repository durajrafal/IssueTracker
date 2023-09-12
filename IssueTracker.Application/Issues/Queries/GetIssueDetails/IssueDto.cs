using IssueTracker.Application.Common.Models;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Enums;

namespace IssueTracker.Application.Issues.Queries.GetIssueDetails
{
    public class IssueDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public PriorityLevel Priority { get; set; }
        public WorkingStatus Status { get; set; }
        public ICollection<Member> Members { get; set; }
        public Project Project { get; set; }
        public AuditDto Audit { get; set; } = new AuditDto();
    }
}
