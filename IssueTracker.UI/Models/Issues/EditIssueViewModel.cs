using IssueTracker.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Models.Issues
{
    public class EditIssueViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public PriorityLevel Priority { get; set; }
        public WorkingStatus Status { get; set; }
        public int ProjectId { get; set; }
    }
}
