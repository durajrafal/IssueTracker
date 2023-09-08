using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Enums;
using IssueTracker.Domain.ValueObjects;

namespace IssueTracker.Application.Issues.Queries.GetIssueDetails
{
    public class IssueDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public PriorityLevel Priority { get; set; }
        public WorkingStatus Status { get; set; }
        public ICollection<Member> Members { get; set; }
        public Project Project { get; set; }
        public DateTime Created { get; set; }
        public User CreatedByUser { get; set; }
        public DateTime? LastModified { get; set; }
        public User? LastModifiedByUser { get; set; }
    }
}
