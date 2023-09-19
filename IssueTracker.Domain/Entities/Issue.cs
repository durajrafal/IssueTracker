using IssueTracker.Domain.Common;
using IssueTracker.Domain.Enums;
using IssueTracker.Domain.ValueObjects;

namespace IssueTracker.Domain.Entities
{
    public class Issue : IAuditableEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public PriorityLevel Priority { get; set; } = PriorityLevel.None;
        public WorkingStatus Status { get; set; } = WorkingStatus.Pending;
        public ICollection<Member> Members { get; init; } = new List<Member>();
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string? LastModifiedById { get; set; }
        public ICollection<AuditEvent> AuditEvents { get; set; } = new List<AuditEvent>();

        public void UpdateMembers(IEnumerable<Member> members, string userId)
        {
            var membersToAdd = members.Except(Members).ToList();
            if (membersToAdd.Count > 0)
            {
                AuditEvents.Add(AuditEvent.CreateCollectionChangeEvent(membersToAdd, 
                    CollectionNames.Members, CollectionOperation.Added, userId));
            }
            foreach (var member in membersToAdd)
            {
                Members.Add(member);
            }

            var membersToRemove = Members.Except(members).ToList();
            if (membersToRemove.Count > 0)
            {
                AuditEvents.Add(AuditEvent.CreateCollectionChangeEvent(membersToRemove, 
                    CollectionNames.Members, CollectionOperation.Removed, userId));
            }
            foreach (var member in membersToRemove)
            {
                Members.Remove(member);
            }
        }
    }
}
