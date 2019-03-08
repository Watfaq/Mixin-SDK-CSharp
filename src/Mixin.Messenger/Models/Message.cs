using System;
using Newtonsoft.Json;

namespace Mixin.Messenger.Models
{
    public class Message
    {
        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("action")] public string Action { get; set; }

        [JsonProperty("data")] public Data Data { get; set; }

        [JsonProperty("error")] public string Error { get; set; }
    }

    public class Data
    {
        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("representative_id")] public string RepresentativeId { get; set; }

        [JsonProperty("quote_message_id")] public string QuoteMessageId { get; set; }

        [JsonProperty("conversation_id")] public string ConversationId { get; set; }

        [JsonProperty("user_id")] public string UserId { get; set; }

        [JsonProperty("message_id")] public string MessageId { get; set; }

        [JsonProperty("category")] public string Category { get; set; }

        [JsonProperty("data")] public string DataData { get; set; }

        [JsonProperty("status")] public string Status { get; set; }

        [JsonProperty("source")] public string Source { get; set; }

        [JsonProperty("created_at")] public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")] public DateTimeOffset UpdatedAt { get; set; }
    }
}