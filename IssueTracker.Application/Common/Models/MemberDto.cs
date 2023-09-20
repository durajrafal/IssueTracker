using IssueTracker.Domain.Entities;
using IssueTracker.Domain.ValueObjects;

namespace IssueTracker.Application.Common.Models
{
    public class MemberDto : IEquatable<MemberDto>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public IEnumerable<ProjectDto>? Projects { get; set; }
        public IEnumerable<IssueDto>? Issues { get; set; }

        public static MemberDto Create(Member member)
        {
            return new MemberDto()
            {
                Id = member.Id,
                UserId = member.UserId,
                User = member.User
            };
        }

        public Member GetEntity()
        {
            return new Member()
            {
                Id = this.Id,
                UserId = this.UserId,
                User = this.User
            };
        }

        public bool Equals(MemberDto other)
        {
            if (other is null)
            {
                return false;
            }
            return UserId == other.UserId;
        }

        public override bool Equals(object obj) => Equals(obj as MemberDto);
        public override int GetHashCode() => (UserId).GetHashCode();
    }
}
