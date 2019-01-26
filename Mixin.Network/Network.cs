using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mixin.Network
{
    public partial class MixinClient
    {
        public string VerifyPin(string pin)
        {
            var encryptedPin = transport.EncryptPin(pin, (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            var body = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"pin", encryptedPin}
            });
            return transport.SendPostRequest("/pin/verify", body);
        }

        public string CreatePin(string oldPin, string newPin)
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

            return transport.SendPostRequest("/pin/update", body);
        }

        public string Deposit(string asset, string accountName = "", string accountTag = "")
        {
            var uri = "/assets/" + asset;
            if (asset == Assets.EOS)
            {
                var body = JsonConvert.SerializeObject(new Dictionary<string, object>
                {
                    {"account_name", accountName},
                    {"account_tag", accountTag}
                });
                return transport.SendPostRequest(uri, body);
            }

            return transport.SendGetRequest(uri);
        }

        public string CreateAddress(string asset, string address, string label, string accountName = "",
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

            return transport.SendPostRequest(uri, JsonConvert.SerializeObject(body));
        }

        public string Withdrawal(string asset, string address, string amount, string label = "")
        {
            var uri = "/withdrawals";
            if (string.IsNullOrEmpty(label)) label = $"Mixin {asset} Address";

            var data = CreateAddress(asset, address, label);
            dynamic json = JObject.Parse(data);
            var addressId = json.data.address_id;
            var body = new Dictionary<string, object>
            {
                {"address_id", addressId},
                {"amount", amount},
                {"pin", transport.EncryptPin(pinCode, (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())},
                {"trace_id", Guid.NewGuid().ToString()},
                {"memo", "Created by Mixin SDK C#"}
            };
            return transport.SendPostRequest(uri, JsonConvert.SerializeObject(body));
        }

        public string DeleteAddress(string addressId)
        {
            var encryptedPin = transport.EncryptPin(pinCode, (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            var body = JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                {"pin", encryptedPin}
            });
            return transport.SendPostRequest($"/addresses/{addressId}/delete", body);
        }

        public string GetAddress(string addressId)
        {
            return transport.SendGetRequest($"/addresses/{addressId}");
        }

        public string ReadAsset(string asset)
        {
            return transport.SendGetRequest($"/assets/{asset}");
        }

        public string ReadAssets()
        {
            return transport.SendGetRequest("/assets");
        }

        public string VerifyPayment(string opponentId, string amount, string asset, string traceId)
        {
            var body = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"asset_id", asset},
                {"opponent_id", opponentId},
                {"amount", amount},
                {"trace_id", traceId}
            });
            return transport.SendPostRequest("/payments", body);
        }

        public string Transfer(string opponentId, string amount, string asset, string memo, string traceId = "")
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

            return transport.SendPostRequest("/transfers", body);
        }

        public string GetTransfer(string traceId)
        {
            return transport.SendGetRequest($"/transfers/trace/{traceId}");
        }

        public string ReadProfile()
        {
            return transport.SendGetRequest("/me");
        }

        public string ExternalTransfer(string assetId, string publicKey, string accountTag, string accountName,
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

            return transport.SendPostRequest("/external/transactions", JsonConvert.SerializeObject(query));
        }

        public string CreateUser(string sessionSecret, string fullName)
        {
            return transport.SendPostRequest("/users", JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                {"session_secret", sessionSecret},
                {"full_name", fullName}
            }));
        }

        public string TopAssets()
        {
            return transport.SendGetRequest("/network");
        }

        public string Snapshots(string offset, string assetId, string order = "DESC", int limit = 100)
        {
            return transport.SendGetRequest("/network/snapshots");
        }

        public string Snapshot(string snapshotId)
        {
            return transport.SendGetRequest($"/network/snapshots/${snapshotId}");
        }
    }
}