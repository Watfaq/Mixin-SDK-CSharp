using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mixin.Network.Models
{
    public class SnapshotListModel
    {
        [JsonProperty("data")] public List<SnapshotData> Data { get; set; }
    }

    public class SnapshotModel
    {
        [JsonProperty("data")] public SnapshotData Data { get; set; }
    }

    public class SnapshotData
    {
        [JsonProperty("amount")] public string Amount { get; set; }

        [JsonProperty("asset")] public AssetData Asset { get; set; }

        [JsonProperty("created_at")] public string CreatedAt { get; set; }

        [JsonProperty("data")] public string Data { get; set; }

        [JsonProperty("snapshot_id")] public string SnapshotId { get; set; }

        [JsonProperty("source")] public string Source { get; set; }

        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("user_id")] public string UserId { get; set; }

        [JsonProperty("trace_id")] public string TraceId { get; set; }

        [JsonProperty("opponent_id")] public string OpponentId { get; set; }
    }
}