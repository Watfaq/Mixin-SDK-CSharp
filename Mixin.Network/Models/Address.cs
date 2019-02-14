namespace Mixin.Network.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class AddressModel
    {
        [JsonProperty("data")]
        public AddressData Data { get; set; }
    }

    public partial class AddressData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("address_id")]
        public Guid AddressId { get; set; }

        [JsonProperty("asset_id")]
        public Guid AssetId { get; set; }

        [JsonProperty("public_key")]
        public string PublicKey { get; set; }

        [JsonProperty("account_name")]
        public string AccountName { get; set; }

        [JsonProperty("account_tag")]
        public string AccountTag { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("fee")]
        public string Fee { get; set; }

        [JsonProperty("reserve")]
        public string Reserve { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}