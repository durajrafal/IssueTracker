using IssueTracker.Domain.Entities;
using IssueTracker.Domain.ValueObjects;

namespace IssueTracker.Application.Projects.Queries.GetProjectDetails
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Member> Members { get; set; }
        public ICollection<Issue> Issues { get; set; }
        public DateTime Created { get; set; }
        public User CreatedByUser { get; set; }
        public DateTime? LastModified { get; set; }
        public User? LastModifiedByUser { get; set; }
    }
}
