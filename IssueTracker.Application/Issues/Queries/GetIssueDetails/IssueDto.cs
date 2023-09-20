using IssueTracker.Domain.Enums;

namespace IssueTracker.Application.Common.Models
{
    public class IssueDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public PriorityLevel Priority { get; set; }
        public WorkingStatus Status { get; set; }
        public IEnumerable<MemberDto> Members { get; set; }
        public ProjectDto Project { get; set; }
        public AuditDto Audit { get; set; } = new AuditDto();
    }
}
