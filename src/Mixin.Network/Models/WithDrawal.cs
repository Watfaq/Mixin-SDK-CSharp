namespace Mixin.Network.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class WithDrawalModel
    {
        [JsonProperty("data")]
        public WithDrawalData Data { get; set; }
    }

    public partial class WithDrawalData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("snapshot_id")]
        public Guid SnapshotId { get; set; }

        [JsonProperty("transaction_hash")]
        public string TransactionHash { get; set; }

        [JsonProperty("asset_id")]
        public Guid AssetId { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("trace_id")]
        public Guid TraceId { get; set; }

        [JsonProperty("memo")]
        public string Memo { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
