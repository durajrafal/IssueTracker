namespace IssueTracker.Domain.ValueObjects
{
    public class AuditEvent
    {
        public int Id { get; set; }
        public string PropertyName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
