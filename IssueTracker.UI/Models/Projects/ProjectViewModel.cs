using IssueTracker.Application.Common.Models;
using IssueTracker.Domain.Enums;

namespace IssueTracker.UI.Models.Projects
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<IssueDto>? Issues { get; set; }
        public string? SelectedStatus { get; set; }
        public string? OrderBy { get; set; }
        public AuditViewModel Audit { get; set; }
        public bool IsPendingSelected { get => SelectedStatus == PENDING; }
        public bool IsInProgressSelected { get => SelectedStatus == IN_PROGRESS; }
        public bool IsCompletedSelected { get => SelectedStatus == COMPLETED; }
        public bool IsAllSelected { get => !IsPendingSelected & !IsInProgressSelected & !IsCompletedSelected; }
        public bool IsPriorityOrderSelected { get => OrderBy == PRIORITY; }
        public bool IsDateCreatedOrderSelected { get => OrderBy == DATE_CREATED; }
        public bool IsDateCreatedDescOrderSelected { get => OrderBy == DATE_CREATED_DESC; }
        public bool IsDateModifiedOrderSelected { get => OrderBy == DATE_MODIFIED; }
        public bool IsDateModifiedDescOrderSelected { get => OrderBy == DATE_MODIFIED_DESC; }

        public bool IsNoneOrderSelected { get => !IsPriorityOrderSelected 
                & !IsDateCreatedOrderSelected & !IsDateCreatedDescOrderSelected
                & !IsDateModifiedOrderSelected & !IsDateModifiedDescOrderSelected; }
        public static string PENDING { get => WorkingStatus.Pending.ToString(); }
        public static string IN_PROGRESS { get => WorkingStatus.InProgress.ToString(); }
        public static string COMPLETED { get => WorkingStatus.Completed.ToString(); }

        public const string PRIORITY = "Priority";
        public const string DATE_CREATED = "Date Created ↑";
        public const string DATE_CREATED_DESC = "Date Created ↓";
        public const string DATE_MODIFIED = "Date Modified ↑";
        public const string DATE_MODIFIED_DESC = "Date Modified ↓";
    }
}
