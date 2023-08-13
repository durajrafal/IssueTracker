using IssueTracker.Domain.Entities;

namespace IssueTracker.UI.Models.Projects
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Issue> Issues { get; init; } = new List<Issue>();

    }
}
