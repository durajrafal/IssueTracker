using IssueTracker.Domain.Enums;
using System.Text.Json;

namespace IssueTracker.Domain.ValueObjects
{
    public class AuditEvent
    {
        public int Id { get; set; }
        public string PropertyName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ModifiedById { get; set; }
        public User ModifiedBy { get; set; }
        public DateTime Timestamp { get; set; }

        public T GetOldValueDeserializedAs<T>()
        {
            return JsonSerializer.Deserialize<T>(OldValue)!;
        }        
        public T GetNewValueDeserializedAs<T>()
        {
            return JsonSerializer.Deserialize<T>(NewValue)!;
        }
        public string GetSummaryText()
        {
            string output = string.Empty;

            switch (PropertyName)
            {
                case "Status":
                    var oldValueDeserialized = GetOldValueDeserializedAs<WorkingStatus>();
                    var newValueDeserialized = GetNewValueDeserializedAs<WorkingStatus>();
                    output = $"{PropertyName} was changed from [{oldValueDeserialized.ToUserFriendlyString()}] to [{newValueDeserialized.ToUserFriendlyString()}].";
                    break;
                case "Priority":
                    output = $"{PropertyName} was changed from [{Enum.Parse(typeof(PriorityLevel), OldValue)}] to [{Enum.Parse(typeof(PriorityLevel), NewValue)}].";
                    break;
                default:
                    output = $"{PropertyName} was changed from [{GetOldValueDeserializedAs<string>()}] to [{GetNewValueDeserializedAs<string>()}].";
                    break;
            }

            return output;
        }
    }
}
