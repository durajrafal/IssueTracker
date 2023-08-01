using IssueTracker.Domain.Models;

namespace IssueTracker.Domain.Entities
{
    public class Member : BaseMember, IEquatable<Member>
    {
        public List<Project> Projects { get; set; }
        public List<Issue> Issues { get; set; }

        public bool Equals(Member other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj) => Equals(obj as Member);
        public override int GetHashCode() => (Id).GetHashCode();
    }
}
