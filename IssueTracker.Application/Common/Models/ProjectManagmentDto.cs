using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Models;

namespace IssueTracker.Application.Common.Models
{
    public class ProjectManagmentDto
    {
        public string Title { get; set; }
        public IEnumerable<ProjectMember> Members { get; set; }
        public IEnumerable<User> OtherUsers { get; set; }
    }
}
