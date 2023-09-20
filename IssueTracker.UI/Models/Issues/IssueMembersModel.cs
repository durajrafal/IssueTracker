using IssueTracker.Application.Common.Models;

namespace IssueTracker.UI.Models.Issues
{
    public class IssueMembersModel
    {
        public int Id { get; set; }
        public IEnumerable<MemberDto> Members { get; set; }
        public IEnumerable<MemberDto> OtherUsers { get; set; }
    }
}
