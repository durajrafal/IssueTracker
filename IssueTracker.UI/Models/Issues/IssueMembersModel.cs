using IssueTracker.Domain.Entities;
using IssueTracker.Domain.ValueObjects;

namespace IssueTracker.UI.Models.Issues
{
    public class IssueMembersModel
    {
        public int Id { get; set; }
        public IEnumerable<Member> Members { get; set; }
        public IEnumerable<Member> OtherUsers { get; set; }
    }
}
