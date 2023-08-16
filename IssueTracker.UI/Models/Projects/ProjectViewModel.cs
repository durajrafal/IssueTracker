using IssueTracker.Domain.Entities;

namespace IssueTracker.UI.Models.Projects
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Issue> Issues { get; init; } = new List<Issue>();
        public string SelectedStatus { get; set; }
        public bool IsOpenedSelected { get => SelectedStatus == OPENED; }
        public bool IsClosedSelected { get => SelectedStatus == CLOSED; }
        public bool IsAllSelected { get => !IsOpenedSelected & !IsClosedSelected; }

        public const string OPENED = "opened";
        public const string CLOSED = "closed";
    }
}
