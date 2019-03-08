using System;
using Newtonsoft.Json;

namespace Mixin.Network.Models
{
    public class TransferModel
    {
        [JsonProperty("data")] public TransferData Data { get; set; }
    }

    public class TransferData
    {
        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("snapshot_id")] public Guid SnapshotId { get; set; }

        [JsonProperty("opponent_id")] public Guid OpponentId { get; set; }

        [JsonProperty("asset_id")] public Guid AssetId { get; set; }

        [JsonProperty("amount")] public string Amount { get; set; }

        [JsonProperty("trace_id")] public Guid TraceId { get; set; }

        [JsonProperty("memo")] public string Memo { get; set; }

        [JsonProperty("created_at")] public DateTimeOffset CreatedAt { get; set; }
    }
}