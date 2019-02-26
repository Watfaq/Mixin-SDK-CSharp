using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mixin.Messenger.Models
{
    public class ConversationModel
    {
        [JsonProperty("data")] public ConversationData Data { get; set; }
    }

    public class ConversationData
    {
        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("conversation_id")] public Guid ConversationId { get; set; }

        [JsonProperty("creator_id")] public Guid CreatorId { get; set; }

        [JsonProperty("category")] public string Category { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("icon_url")] public string IconUrl { get; set; }

        [JsonProperty("announcement")] public string Announcement { get; set; }

        [JsonProperty("created_at")] public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("code_id")] public Guid CodeId { get; set; }

        [JsonProperty("code_url")] public Uri CodeUrl { get; set; }

        [JsonProperty("mute_until")] public DateTimeOffset MuteUntil { get; set; }

        [JsonProperty("participants")] public List<Participant> Participants { get; set; }
    }

    public class Participant
    {
        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("user_id")] public Guid UserId { get; set; }

        [JsonProperty("role")] public string Role { get; set; }

        [JsonProperty("created_at")] public DateTimeOffset CreatedAt { get; set; }
    }
}