namespace Mixin.Network.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class TransactionModel
    {
        [JsonProperty("data")]
        public List<TransactionData> Data { get; set; }
    }

    public partial class TransactionData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("transaction_id")]
        public Guid TransactionId { get; set; }

        [JsonProperty("transaction_hash")]
        public string TransactionHash { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("public_key")]
        public string PublicKey { get; set; }

        [JsonProperty("account_name")]
        public string AccountName { get; set; }

        [JsonProperty("account_tag")]
        public string AccountTag { get; set; }

        [JsonProperty("asset_id")]
        public Guid AssetId { get; set; }

        [JsonProperty("chain_id")]
        public Guid ChainId { get; set; }

        [JsonProperty("confirmations")]
        public long Confirmations { get; set; }

        [JsonProperty("threshold")]
        public long Threshold { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
