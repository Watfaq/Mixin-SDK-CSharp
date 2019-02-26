using System;
using Newtonsoft.Json;

namespace Mixin.Messenger.Models
{
    public class AttachmentModel
    {
        [JsonProperty("data")] public AttachmentData AttachmentData { get; set; }
    }

    public class AttachmentData
    {
        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("attachment_id")] public Guid AttachmentId { get; set; }

        [JsonProperty("upload_url")] public Uri UploadUrl { get; set; }

        [JsonProperty("view_url")] public Uri ViewUrl { get; set; }
    }
}