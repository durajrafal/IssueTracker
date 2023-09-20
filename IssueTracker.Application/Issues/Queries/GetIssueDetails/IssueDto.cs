using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Domain.Entities;
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
        public IEnumerable<MemberDto>? Members { get; set; }
        public ProjectDto Project { get; set; }
        public AuditDto Audit { get; set; } = new AuditDto();

        public static IssueDto Create(Issue issue, IUserService userService)
        {
            return new IssueDto()
            {
                Id = issue.Id,
                Title = issue.Title,
                Description = issue.Description,
                Priority = issue.Priority,
                Status = issue.Status,
                Members = issue.Members?.Select(x => MemberDto.Create(x)),
                Project = ProjectDto.Create(issue.Project),
                Audit = AuditDto.Create(issue, userService)
            };
        }
    }
}
