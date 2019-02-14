using System;
using System.Collections.Generic;
using Mixin.Network.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mixin.Network
{
    public partial class MixinClient
    {
        public UserData VerifyPin(string pin)
        {
            var encryptedPin = transport.EncryptPin(pin, (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            var body = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"pin", encryptedPin}
            });
            return JsonConvert.DeserializeObject<UserModel>(transport.SendPostRequest("/pin/verify", body)).Data;
        }

        public UserData CreatePin(string oldPin, string newPin)
        {
            var oldEncryptedPin = string.Empty;
            if (oldPin.Length > 0)
                oldEncryptedPin = transport.EncryptPin(oldPin, (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

            var newEncryptedPin = transport.EncryptPin(newPin, (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            var body = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"old_pin", oldEncryptedPin},
                {"pin", newEncryptedPin}
            });

            return JsonConvert.DeserializeObject<UserModel>(transport.SendPostRequest("/pin/update", body)).Data;
        }

        public AssetData Deposit(string asset, string accountName = "", string accountTag = "")
        {
            var uri = "/assets/" + asset;
            if (asset == Assets.EOS)
            {
                var body = JsonConvert.SerializeObject(new Dictionary<string, object>
                {
                    {"account_name", accountName},
                    {"account_tag", accountTag}
                });
                return JsonConvert.DeserializeObject<AssetModel>(transport.SendPostRequest(uri, body)).Data;
            }

            return JsonConvert.DeserializeObject<AssetModel>(transport.SendGetRequest(uri)).Data;
        }

        public AddressData CreateAddress(string asset, string address, string label, string accountName = "",
            string accountTag = "")
        {
            var uri = "/addresses";
            var pin = transport.EncryptPin(pinCode, (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            var body = new Dictionary<string, object>
            {
                {"asset_id", asset},
                {"public_key", address},
                {"label", label},
                {"pin", pin}
            };
            if (asset == Assets.EOS)
            {
                body.Add("account_name", accountName);
                body.Add("account_tag", accountTag);
            }

            return JsonConvert.DeserializeObject<AddressModel>(transport.SendPostRequest(uri, JsonConvert.SerializeObject(body))).Data;
        }

        public WithDrawalData Withdrawal(string asset, string address, string amount, string label = "")
        {
            var uri = "/withdrawals";
            if (string.IsNullOrEmpty(label)) label = $"Mixin {asset} Address";

            var data = CreateAddress(asset, address, label);
            var addressId = data.AddressId;
            var body = new Dictionary<string, object>
            {
                {"address_id", addressId},
                {"amount", amount},
                {"pin", transport.EncryptPin(pinCode, (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())},
                {"trace_id", Guid.NewGuid().ToString()},
                {"memo", "Created by Mixin SDK C#"}
            };
            return JsonConvert.DeserializeObject<WithDrawalModel>(transport.SendPostRequest(uri, JsonConvert.SerializeObject(body))).Data;
        }

        public void DeleteAddress(string addressId)
        {
            var encryptedPin = transport.EncryptPin(pinCode, (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            var body = JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                {"pin", encryptedPin}
            });
            transport.SendPostRequest($"/addresses/{addressId}/delete", body);
        }

        public AddressData GetAddress(string addressId)
        {
            return JsonConvert.DeserializeObject<AddressModel>(transport.SendGetRequest($"/addresses/{addressId}")).Data;
        }

        public AssetData ReadAsset(string asset)
        {
            return JsonConvert.DeserializeObject<AssetModel>(transport.SendGetRequest($"/assets/{asset}")).Data;
        }

        public List<AssetData> ReadAssets()
        {
            return JsonConvert.DeserializeObject<AssetListModel>(transport.SendGetRequest("/assets")).Data;
        }

        public VerifyPamentData VerifyPayment(string opponentId, string amount, string asset, string traceId)
        {
            var body = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"asset_id", asset},
                {"opponent_id", opponentId},
                {"amount", amount},
                {"trace_id", traceId}
            });
            return JsonConvert.DeserializeObject<VerifyPamentModel>(transport.SendPostRequest("/payments", body)).Data;
        }

        public TransferData Transfer(string opponentId, string amount, string asset, string memo, string traceId = "")
        {
            var pin = transport.EncryptPin(pinCode, (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            if (string.IsNullOrEmpty(traceId)) traceId = Guid.NewGuid().ToString();

            var body = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"asset_id", asset},
                {"opponent_id", opponentId},
                {"amount", amount},
                {"pin", pin},
                {"trace_id", traceId},
                {"memo", memo}
            });

            return JsonConvert.DeserializeObject<TransferModel>(transport.SendPostRequest("/transfers", body)).Data;
        }

        public TransferData GetTransfer(string traceId)
        {
            return JsonConvert.DeserializeObject<TransferModel>(transport.SendGetRequest($"/transfers/trace/{traceId}")).Data;
        }

        public List<TransactionData> ExternalTransfer(string assetId, string publicKey, string accountTag, string accountName,
            string limit,
            string offset)
        {
            var query = new Dictionary<string, string>
            {
                {"asset", assetId},
                {"public_key", publicKey},
                {"account_tag", accountTag},
                {"account_name", accountName},
                {"limit", limit},
                {"offset", offset}
            };

            return JsonConvert.DeserializeObject<TransactionModel>(transport.SendPostRequest("/external/transactions", JsonConvert.SerializeObject(query))).Data;
        }

        public UserData CreateUser(string sessionSecret, string fullName)
        {
            return JsonConvert.DeserializeObject<UserModel>(transport.SendPostRequest("/users", JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                {"session_secret", sessionSecret},
                {"full_name", fullName}
            }))).Data;
        }

        public List<AssetData> TopAssets()
        {
            return JsonConvert.DeserializeObject<AssetListModel>(transport.SendGetRequest("/network")).Data;
        }

        public List<SnapshotData> Snapshots(string offset, string assetId, string order = "DESC", int limit = 100)
        {
            return JsonConvert.DeserializeObject<SnapshotListModel>(transport.SendGetRequest("/network/snapshots")).Data;
        }

        public SnapshotData Snapshot(string snapshotId)
        {
            return JsonConvert.DeserializeObject<SnapshotModel>(transport.SendGetRequest($"/network/snapshots/${snapshotId}")).Data;
        }
    }
}