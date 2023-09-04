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
}
