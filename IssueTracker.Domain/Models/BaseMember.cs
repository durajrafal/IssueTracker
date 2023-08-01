namespace IssueTracker.Domain.Models
{
    public abstract class BaseMember
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
