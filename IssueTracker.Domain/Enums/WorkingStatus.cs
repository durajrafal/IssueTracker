using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Domain.Enums
{
    public enum WorkingStatus
    {
        Pending = 0,
        [Display(Name = "In Progress")]
        InProgress = 1,
        Completed = 2
    }

    public static class WorkingStatusExtensions
    {
        public static string ToUserFriendlyString(this WorkingStatus status)
        {
            switch (status)
            {
                case WorkingStatus.Pending:
                    return "Pending";
                case WorkingStatus.InProgress:
                    return "In Progress";
                case WorkingStatus.Completed:
                    return "Completed";
                default:
                    return "";
            }
        }
    }
}
