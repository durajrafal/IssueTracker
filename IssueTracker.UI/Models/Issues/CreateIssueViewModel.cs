using IssueTracker.Domain.Entities;

namespace IssueTracker.UI.Models.Issues
{
    public class CreateIssueViewModel
    {
        public int ProjectId { get; init; }
        public ICollection<Member> Members { get; set; }
    }
}
