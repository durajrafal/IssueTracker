using IssueTracker.Domain.Common;

namespace IssueTracker.Domain.Entities
{
    public class Project : AuditableEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Member> Members { get; init; } = new List<Member>();
        public ICollection<Issue> Issues { get; init; } = new List<Issue>();
    }
}
