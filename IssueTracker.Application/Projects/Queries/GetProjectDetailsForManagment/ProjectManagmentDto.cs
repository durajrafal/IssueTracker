using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Models;

namespace IssueTracker.Application.Projects.Queries.GetProjectDetailsForManagment
{
    public class ProjectManagmentDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<ProjectMember> Members { get; set; }
        public IEnumerable<User> OtherUsers { get; set; }
    }
}
