using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mixin.Network.Models
{
    public class AssetModel
    {
        [JsonProperty("data")] public AssetData Data { get; set; }
    }

    public class AssetListModel
    {
        [JsonProperty("data")] public List<AssetData> Data { get; set; }
    }


    public class AssetData
    {
        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("asset_id")] public Guid AssetId { get; set; }

        [JsonProperty("chain_id")] public Guid ChainId { get; set; }

        [JsonProperty("symbol")] public string Symbol { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("icon_url")] public Uri IconUrl { get; set; }

        [JsonProperty("balance")] public string Balance { get; set; }

        [JsonProperty("public_key")] public string PublicKey { get; set; }

        [JsonProperty("account_name")] public string AccountName { get; set; }

        [JsonProperty("account_tag")] public string AccountTag { get; set; }

        [JsonProperty("price_btc")] public string PriceBtc { get; set; }

        [JsonProperty("price_usd")] public string PriceUsd { get; set; }

        [JsonProperty("change_btc")] public string ChangeBtc { get; set; }

        [JsonProperty("change_usd")] public string ChangeUsd { get; set; }

        [JsonProperty("asset_key")] public string AssetKey { get; set; }

        [JsonProperty("confirmations")] public long Confirmations { get; set; }

        [JsonProperty("capitalization")] public double Capitalization { get; set; }
    }
}