using IssueTracker.Domain.Entities;
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
        public List<SelectListItem> ProjectMembersSelecList { get; init; }
        public IEnumerable<string> AssignedMembersId { get; set; }

        public CreateIssueViewModel()
        {
            
        }

        public CreateIssueViewModel(ICollection<Member> members)
        {
            ProjectMembersSelecList = new List<SelectListItem>();
            members.ToList().ForEach(x =>
            {
                ProjectMembersSelecList.Add(new SelectListItem
                {
                    Text = $"{x.User.FullName} ({x.User.Email})",
                    Value = x.UserId
                });
            });
        }
    }
}
