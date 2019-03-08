using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Mixin.Network;
using Newtonsoft.Json;
using Websocket.Client;

namespace Mixin.Messenger
{
    public class WebSocketClient
    {
        public delegate void OnCloseDelegate(object sender, DisconnectionType t);

        public delegate void OnMessageDelegate(object sender, MessageType args, string message);

        public delegate void OnOpenDelegate(object sender, EventArgs args);

        public delegate void OnReconnectionDelegate(object sender, ReconnectionType t);

        private readonly WebsocketClient ws;

        private readonly string clientId;
        public OnCloseDelegate onCloseDelegate;
        public OnMessageDelegate onMessageDelegate;
        public OnOpenDelegate onOpenDelegate;
        public OnReconnectionDelegate onReconnectionDelegate;

        public WebSocketClient(string clientId, string sessionId, string privateKey,
            OnMessageDelegate onMessageDelegate = null, OnOpenDelegate onOpenDelegate
                = null, OnCloseDelegate onCloseDelegate = null, OnReconnectionDelegate onReconnectionDelegate = null)
        {
            this.onMessageDelegate = onMessageDelegate;
            this.onOpenDelegate = onOpenDelegate;
            this.onCloseDelegate = onCloseDelegate;
            this.onReconnectionDelegate = onReconnectionDelegate;
            this.clientId = clientId;

            var transport = new MixinClientTransport(clientId, sessionId, "", privateKey);

            ws = new WebsocketClient(new Uri("wss://blaze.mixin.one/"), () =>
            {
                var client = new ClientWebSocket();
                client.Options.AddSubProtocol("Mixin-Blaze-1");
                client.Options.SetRequestHeader("Authorization", "Bearer " + transport.SignAuthToken("GET", "/"));
                return client;
            });

            ws.ReconnectTimeoutMs = (int) TimeSpan.FromHours(1).TotalMilliseconds;
            ws.ReconnectionHappened.Subscribe(type => OnReconnection(ws, type));
            ws.DisconnectionHappened.Subscribe(type => OnClose(ws, type));
            ws.MessageReceived.Subscribe(msg => OnMessage(ws, msg));
        }

        private void OnReconnection(object sender, ReconnectionType e)
        {
            onReconnectionDelegate?.Invoke(sender, e);
        }

        private void OnClose(object sender, DisconnectionType e)
        {
            onCloseDelegate?.Invoke(sender, e);
        }

        public async Task Run()
        {
            await ws.Start();
            OnOpen(ws, null);
        }

        public void WriteMessage(string action, Dictionary<string, object> body = null)
        {
            var message = new Dictionary<string, object>
            {
                {"id", Guid.NewGuid().ToString()},
                {"action", action}
            };
            if (body != null) message.Add("params", body);

            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                    gzipStream.Write(bytes, 0, bytes.Length);
                }

                ws.SendInstant(outputStream.ToArray());
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


        private void OnMessage(object sender, MessageType msg)
        {
            using (var msi = new MemoryStream(msg.RawData))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    CopyTo(gs, mso);
                }

                onMessageDelegate?.Invoke(sender, msg, Encoding.UTF8.GetString(mso.ToArray()));
            }
        }

        private static void CopyTo(Stream src, Stream dest)
        {
            var bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) dest.Write(bytes, 0, cnt);
        }

        public string Base64Decode(string s)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(s));
        }
    }
}