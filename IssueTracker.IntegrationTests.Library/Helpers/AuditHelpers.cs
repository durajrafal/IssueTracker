using IssueTracker.Domain.ValueObjects;

namespace IssueTracker.IntegrationTests.Library.Helpers
{
    public static class AuditHelpers
    {

        public static AuditEvent DeserializeValuesPropertiesAsString(this AuditEvent auditEvent)
        {
            auditEvent.OldValue = auditEvent.GetOldValueDeserializedAs<string>();
            auditEvent.NewValue = auditEvent.GetNewValueDeserializedAs<string>();
            return auditEvent;
        }
    }
}
