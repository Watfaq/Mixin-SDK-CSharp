using System;
using Newtonsoft.Json;

namespace Mixin.Network.Models
{
    public class VerifyPamentModel
    {
        [JsonProperty("data")] public VerifyPamentData Data { get; set; }
    }

    public class VerifyPamentData
    {
        [JsonProperty("recipient")] public RecipientData Recipient { get; set; }

        [JsonProperty("asset")] public AssetData Asset { get; set; }
    }

    public class RecipientData
    {
        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("user_id")] public Guid UserId { get; set; }

        [JsonProperty("identity_number")] public string IdentityNumber { get; set; }

        [JsonProperty("full_name")] public string FullName { get; set; }

        [JsonProperty("avatar_url")] public string AvatarUrl { get; set; }

        [JsonProperty("relationship")] public string Relationship { get; set; }

        [JsonProperty("mute_until")] public DateTimeOffset MuteUntil { get; set; }

        [JsonProperty("created_at")] public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("is_verified")] public bool IsVerified { get; set; }
    }
}