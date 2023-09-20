using IssueTracker.Application.Common.Models;
using IssueTracker.Domain.Enums;

namespace IssueTracker.UI.Models.Issues
{
    public class IssueViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public PriorityLevel Priority { get; set; }
        public WorkingStatus Status { get; set; }
        public IEnumerable<MemberDto> Members { get; set; }
        public ProjectDto Project { get; set; }
        public AuditViewModel Audit { get; set; }
    }
}
