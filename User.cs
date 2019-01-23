using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using Jose;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace Mixin.Network
{
    public partial class User
    {
        public User(string clientId, string sessionId, string pinToken, string pinCode, string privateKey)
        {
            this.clientId = clientId;
            this.sessionId = sessionId;
            this.pinToken = pinToken;
            this.pinCode = pinCode;
            this.privateKey = privateKey;
        }

        private string clientId;
        private string sessionId;
        private string pinToken;
        private string pinCode;
        private string privateKey;
        
        private string baseUrl = "https://api.mixin.one";
        

        public string VerifyPin(string pin)
        {
            var encryptedPin = encryptPin(pin, (UInt64)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            var body = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"pin", encryptedPin}
            });
            return sendPostRequest("/pin/verify", body);
        }

        public string CreatePin(string oldPin, string newPin)
        {
            var oldEncryptedPin = string.Empty;
            if (oldPin.Length > 0)
            {
                oldEncryptedPin = encryptPin(oldPin, (UInt64) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            }

            var newEncryptedPin = encryptPin(newPin, (UInt64) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            var body = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"old_pin", oldEncryptedPin},
                {"pin", newEncryptedPin}
            });

            return sendPostRequest("/pin/update", body);
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
                return sendPostRequest(uri, body);
            }

            return sendGetRequest(uri);
        }

        public string CreateAddress(string asset, string address, string label, string accountName = "",
            string accountTag = "")
        {
            var uri = "/addresses";
            var pin = encryptPin(pinCode, (UInt64) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
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

            return sendPostRequest(uri, JsonConvert.SerializeObject(body));
        }

        public string Withdrawal(string asset, string address, string amount, string label = "")
        {
            var uri = "/withdrawals";
            if (string.IsNullOrEmpty(label))
            {
                label = $"Mixin {asset} Address";
            }

            var data = CreateAddress(asset, address, label);
            dynamic json = JObject.Parse(data);
            var addressId = json.data.address_id;
            var body = new Dictionary<string, object>
            {
                {"address_id", addressId},
                {"amount", amount},
                {"pin", encryptPin(pinCode, (UInt64) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())},
                {"trace_id", Guid.NewGuid().ToString()},
                {"memo", "Created by Mixin SDK C#"}
            };
            return sendPostRequest(uri, JsonConvert.SerializeObject(body));
        }

        public string ReadAsset(string asset)
        {
            return sendGetRequest($"/assets/{asset}");
        }

        public string ReadAssets()
        {
            return sendGetRequest("/assets");
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
            return sendPostRequest("/payments", body);
        }

        public string Transfer(string opponentId, string amount, string asset, string memo, string traceId = "")
        {
            var pin = encryptPin(pinCode, (UInt64) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            if (string.IsNullOrEmpty(traceId))
            {
                traceId = Guid.NewGuid().ToString();
            }

            var body = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"asset_id", asset},
                {"opponent_id", opponentId},
                {"amount", amount},
                {"pin", pin},
                {"trace_id", traceId},
                {"memo", memo}
            });

            return sendPostRequest("/transfers", body);
        }

        public string ReadProfile()
        {
            return sendGetRequest("/me");
        }




        
    }
}