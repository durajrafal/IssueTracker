namespace IssueTracker.Domain.Models
{
    public abstract class Member
    {
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
