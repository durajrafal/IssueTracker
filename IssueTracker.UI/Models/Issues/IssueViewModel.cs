using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Enums;

namespace IssueTracker.UI.Models.Issues
{
    public class IssueViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public PriorityLevel Priority { get; set; }
        public WorkingStatus Status { get; set; }
        public ICollection<Member> Members { get; set; }
        public Project Project { get; set; }
    }
}
