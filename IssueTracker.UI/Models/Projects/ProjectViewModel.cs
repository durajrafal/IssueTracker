using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Enums;

namespace IssueTracker.UI.Models.Projects
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Issue> Issues { get; set; } = new List<Issue>();
        public string? SelectedStatus { get; set; }
        public string? OrderBy { get; set; }
        public AuditViewModel Audit { get; set; }
        public bool IsPendingSelected { get => SelectedStatus == PENDING; }
        public bool IsInProgressSelected { get => SelectedStatus == IN_PROGRESS; }
        public bool IsCompletedSelected { get => SelectedStatus == COMPLETED; }
        public bool IsAllSelected { get => !IsPendingSelected & !IsInProgressSelected & !IsCompletedSelected; }
        public bool IsPriorityOrderSelected { get => OrderBy == PRIORITY; }
        public bool IsDateOrderSelected { get => OrderBy == DATE; }

        public bool IsNoneOrderSelected { get => !IsPriorityOrderSelected & !IsDateOrderSelected; }
        public static string PENDING { get => WorkingStatus.Pending.ToString(); }
        public static string IN_PROGRESS { get => WorkingStatus.InProgress.ToString(); }
        public static string COMPLETED { get => WorkingStatus.Completed.ToString(); }

        public const string PRIORITY = "Priority";
        public const string DATE = "Date";
    }
}
