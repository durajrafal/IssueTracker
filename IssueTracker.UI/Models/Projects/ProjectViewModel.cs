using IssueTracker.Domain.Entities;

namespace IssueTracker.UI.Models.Projects
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Issue> Issues { get; init; } = new List<Issue>();
        public string SelectedStatus { get; set; }
        public bool IsPendingSelected { get => SelectedStatus == PENDING; }
        public bool IsInProgressSelected { get => SelectedStatus == IN_PROGRESS; }
        public bool IsCompletedSelected { get => SelectedStatus == COMPLETED; }
        public bool IsAllSelected { get => !IsPendingSelected & !IsInProgressSelected & !IsCompletedSelected; }

        public const string PENDING = nameof(PENDING);
        public const string IN_PROGRESS = nameof(IN_PROGRESS);
        public const string COMPLETED = nameof(COMPLETED);
    }
}
