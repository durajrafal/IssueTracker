using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

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
                case nameof(Issue.Status):
                    var oldValueDeserialized = GetOldValueDeserializedAs<WorkingStatus>();
                    var newValueDeserialized = GetNewValueDeserializedAs<WorkingStatus>();
                    output = $"{PropertyName} was changed from [{oldValueDeserialized.ToUserFriendlyString()}] to [{newValueDeserialized.ToUserFriendlyString()}].";
                    break;
                case nameof(Issue.Priority):
                    output = $"{PropertyName} was changed from [{Enum.Parse(typeof(PriorityLevel), OldValue)}] to [{Enum.Parse(typeof(PriorityLevel), NewValue)}].";
                    break;
                case nameof(CollectionNames.Members):
                    var members = GetNewValueDeserializedAs<IEnumerable<Member>>();
                    output = $"{OldValue} members: [{string.Join(", ", members.Select(x => x.User.FullName))}]";
                    break;
                default:
                    output = $"{PropertyName} was changed from [{GetOldValueDeserializedAs<string>()}] to [{GetNewValueDeserializedAs<string>()}].";
                    break;
            }

            return output;
        }

        public static AuditEvent CreateCollectionChangeEvent<T>(IEnumerable<T> itemsChanged, CollectionNames collectionName, CollectionOperation operation, string userId)
        {
            return new AuditEvent()
            {
                PropertyName = collectionName.ToString(),
                OldValue = operation.ToString(),
                NewValue = JsonSerializer.Serialize(itemsChanged, 
                    new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles}),
                ModifiedById = userId,
                Timestamp = DateTime.UtcNow
            };
        }
    }

    public enum CollectionNames
    {
        Members,
        Comments,
        Issues
    }

    public enum CollectionOperation
    {
        Added,
        Removed
    }
}
