using IssueTracker.Application.Common.Models;
using IssueTracker.Domain.Entities;

namespace IssueTracker.Application.Projects.Queries.GetProjectDetails
{
    public class ProjectDto : AuditableEntityDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Member> Members { get; set; }
        public ICollection<Issue> Issues { get; set; }
    }
}
