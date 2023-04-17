using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Models;

namespace IssueTracker.UI.Models.Projects
{
    public class ProjectManagmentViewModel
    {
        public string Title { get; set; }
        public IEnumerable<ProjectMember> Members { get; set; }
        public IEnumerable<User> OtherUsers { get; set; }
    }
}
