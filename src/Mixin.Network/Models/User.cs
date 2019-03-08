using System;
using Newtonsoft.Json;

namespace Mixin.Network.Models
{
    public class UserModel
    {
        [JsonProperty("data")] public UserData Data { get; set; }
    }

    public class UserData
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

        [JsonProperty("session_id")] public Guid SessionId { get; set; }

        [JsonProperty("phone")] public string Phone { get; set; }

        [JsonProperty("pin_token")] public string PinToken { get; set; }

        [JsonProperty("invitation_code")] public string InvitationCode { get; set; }

        [JsonProperty("code_id")] public Guid CodeId { get; set; }

        [JsonProperty("code_url")] public Uri CodeUrl { get; set; }

        [JsonProperty("has_pin")] public bool HasPin { get; set; }

        [JsonProperty("receive_message_source")]
        public string ReceiveMessageSource { get; set; }
    }
}