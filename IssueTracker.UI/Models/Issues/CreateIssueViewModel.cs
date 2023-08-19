using IssueTracker.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IssueTracker.UI.Models.Issues
{
    public class CreateIssueViewModel
    {
        public int ProjectId { get; init; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public PriorityLevel Priority { get; set; }
        public List<SelectListItem> ProjectMembersSelecList { get; set; }
        public IEnumerable<string> AssignedMembersId { get; set; }
    }
}
