using IssueTracker.Domain.Entities;

namespace IssueTracker.Application.Common.Models
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<MemberDto>? Members { get; set; }
        public IEnumerable<IssueDto>? Issues { get; set; }
        public AuditDto Audit { get; set; } = new AuditDto();

        public static ProjectDto Create(Project project)
        {
            return new ProjectDto()
            {
                Id = project.Id,
                Title = project.Title,
                Members = project.Members?.Select(x => MemberDto.Create(x)),
            };
        }
    }
}
