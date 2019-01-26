#region

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using Jose;
using Mixin.Network;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using WebSocketSharp;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

#endregion

namespace Mixin.Messenger
{
    public class WebSocketClient
    {
        public delegate void OnCloseDelegate(object sender, EventArgs args);

        public delegate void OnErrorDelegate(object sender, EventArgs args);

        public delegate void OnMessageDelegate(object sender, EventArgs args, string message);

        public delegate void OnOpenDelegate(object sender, EventArgs args);

        private readonly WebSocket ws;
        public OnCloseDelegate onCloseDelegate;
        public OnErrorDelegate onErrorDelegate;
        public OnMessageDelegate onMessageDelegate;
        public OnOpenDelegate onOpenDelegate;

        private string clientId;

        public WebSocketClient(string clientId, string sessionId, string privateKey,
            OnMessageDelegate onMessageDelegate = null, OnOpenDelegate onOpenDelegate
                = null, OnCloseDelegate onCloseDelegate = null, OnErrorDelegate onErrorDelegate = null)
        {

            this.onMessageDelegate = onMessageDelegate;
            this.onOpenDelegate = onOpenDelegate;
            this.onCloseDelegate = onCloseDelegate;
            this.onErrorDelegate = onErrorDelegate;
            this.clientId = clientId;

            var transport = new MixinClientTransport(clientId, sessionId, "", privateKey);

            ws = new WebSocket("wss://blaze.mixin.one/", "Mixin-Blaze-1")
            {
                CustomHeaders =
                    new Dictionary<string, string> {{"Authorization", $"Bearer {transport.SignAuthToken("GET", "/")}"}},
                SslConfiguration = {EnabledSslProtocols = SslProtocols.Tls12}
            };

            ws.OnOpen += OnOpen;
            ws.OnMessage += OnMessage;
            ws.OnClose += OnClose;
            ws.OnError += OnError;
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            onErrorDelegate?.Invoke(sender, e);
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            onCloseDelegate?.Invoke(sender, e);
        }

        public void Run()
        {
            ws.Connect();
        }

        public void WriteMessage(string action, Dictionary<string, object> body = null)
        {
            var message = new Dictionary<string, object>
            {
                {"id", Guid.NewGuid().ToString()},
                {"action", action}
            };
            if (body != null)
            {
                message.Add("params", body);
            }

            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                    gzipStream.Write(bytes, 0, bytes.Length);
                }

                ws.Send(outputStream.ToArray());
            }
        }

        public void AckMessage(string messageId)
        {
            var param = new Dictionary<string, object>
            {
                {"message_id", messageId},
                {"status", "READ"}
            };
            WriteMessage("ACKNOWLEDGE_MESSAGE_RECEIPT", param);
        }

        public void SendUserAppButton(string conversationId, string toUserId, string linkHref, string linkText,
            string linkColor = "#000")
        {
            var buttonInfo = new Dictionary<string, string>
            {
                {"label", linkText},
                {"action", linkHref},
                {"color", linkColor}
            };
            var button = JsonConvert.SerializeObject(new List<Dictionary<string, string>> {buttonInfo});
            WriteMessage("CREATE_MESSAGE", new Dictionary<string, object>
            {
                {"conversation_id", conversationId},
                {"recipient_id", toUserId},
                {"message_id", Guid.NewGuid().ToString()},
                {"category", "APP_BUTTON_GROUP"},
                {"data", Convert.ToBase64String(Encoding.UTF8.GetBytes(button))}
            });
        }

        public void SendUserContactCard(string conversationId, string toUserId, string contactUserId)
        {
            var card = JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                {"user_id", contactUserId}
            });
            WriteMessage("CREATE_MESSAGE", new Dictionary<string, object>
            {
                {"conversation_id", conversationId},
                {"recipient_id", toUserId},
                {"message_id", Guid.NewGuid().ToString()},
                {"category", "PLAIN_CONTACT"},
                {"data", Convert.ToBase64String(Encoding.UTF8.GetBytes(card))}
            });
        }

        public void SendUserPlainText(string conversationId, string toUserId, string textContent)
        {
            WriteMessage("CREATE_MESSAGE", new Dictionary<string, object>
            {
                {"conversation_id", conversationId},
                {"recipient_id", toUserId},
                {"status", "SENT"},
                {"message_id", Guid.NewGuid().ToString()},
                {"category", "PLAIN_TEXT"},
                {"data", Convert.ToBase64String(Encoding.UTF8.GetBytes(textContent))}
            });
        }

        public void SendUserPayAppButton(string conversationId, string toUserId, string assetName, string assetId,
            string payAmount, string linkColor = "#00f")
        {
            var payLink =
                $"https://mixin.one/pay?recipient={clientId}&asset={assetId}&amount={payAmount}&trace={Guid.NewGuid().ToString()}&memo=BotPayment";
            SendUserAppButton(conversationId, toUserId, payLink, $"Pay {payAmount} {assetName}", linkColor);
        }

        private void OnOpen(object sender, EventArgs args)
        {
            if (onOpenDelegate != null)
            {
                onOpenDelegate.Invoke(sender, args);
                return;
            }

            WriteMessage("LIST_PENDING_MESSAGES");
        }


        private void OnMessage(object sender, EventArgs args)
        {
            var msgEvtArgs = (MessageEventArgs) args;
            using (var msi = new MemoryStream(msgEvtArgs.RawData))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    CopyTo(gs, mso);
                }

                onMessageDelegate?.Invoke(sender, args, Encoding.UTF8.GetString(mso.ToArray()));
            }
        }

        private static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }
     
        public string Base64Decode(string s)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(s));
        }
    }
}