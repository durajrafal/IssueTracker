using IssueTracker.Domain.Models;

namespace IssueTracker.Domain.Entities
{
    public class Member : IEquatable<Member>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<Project> Projects { get; set; }
        public ICollection<Issue> Issues { get; set; }

        public bool Equals(Member other)
        {
            return UserId == other.UserId;
        }

        public override bool Equals(object obj) => Equals(obj as Member);
        public override int GetHashCode() => (UserId).GetHashCode();
    }
}
