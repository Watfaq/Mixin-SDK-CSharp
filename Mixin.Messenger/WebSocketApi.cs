using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Jose;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using WebSocketSharp;


namespace Mixin.Messenger
{
    public class WebSocketApi
    {
        private string clientId;
        private string sessionId;
        private string privateKey;
        private static OnMessageDelegate _onMessageDelegate;

        private WebSocket ws;

        public delegate void OnMessageDelegate(object sender, EventArgs args, string message);
        
        public WebSocketApi(string clientId, string sessionId, string privateKey, OnMessageDelegate onMessageDelegate=null)
        {
            this.clientId = clientId;
            this.sessionId = sessionId;
            this.privateKey = privateKey;

            _onMessageDelegate = onMessageDelegate;

            ws = new WebSocket("wss://blaze.mixin.one/", new []{"Mixin-Blaze-1"})
            {
                CustomHeaders = new Dictionary<string, string>
                {
                    {"Authorization", $"Bearer {signAuthToken("GET", "/")}"}
                }
            };
            
            ws.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
            ws.OnOpen += OnOpen;
            ws.OnMessage += OnMessage;
        }

        public void Run()
        {
            ws.Connect();
            Thread.Sleep(Timeout.Infinite);
        }

        private static void OnOpen(object sender, EventArgs args)
        {
            var ws = (WebSocket) sender;
            var message = JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                {"id", Guid.NewGuid().ToString()},
                {"action", "LIST_PENDING_MESSAGES"}
            });
            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    var bytes = Encoding.UTF8.GetBytes(message);
                    gzipStream.Write(bytes, 0, bytes.Length);
                }

                ws.Send(outputStream.ToArray());

            }

        }
        
        

        private static void OnMessage(object sender, EventArgs args)
        {
            var msgEvtArgs = (MessageEventArgs) args;
            using (var msi = new MemoryStream(msgEvtArgs.RawData))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    CopyTo(gs,mso);
                }
                if (_onMessageDelegate != null)
                {
                    _onMessageDelegate(sender, args, Encoding.UTF8.GetString(mso.ToArray()));
                }
            }
        }
        
        private static void CopyTo(Stream src, Stream dest) {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) {
                dest.Write(bytes, 0, cnt);
            }
        }

        
        private string signAuthToken(string method, string uri, string body="")
        {
            var iat = DateTimeOffset.UtcNow;
            var exp = iat + TimeSpan.FromDays(30);
            var sig = sha256(method+ uri+ body);
            var claims = new Dictionary<string, object>
            {
                {"uid", clientId},
                {"sid", sessionId},
                {"iat", iat.ToUnixTimeSeconds()},
                {"exp", exp.ToUnixTimeSeconds()},
                {"jti", Guid.NewGuid().ToString()},
                {"sig", sig}
            };


            var pr = new PemReader(new StringReader(privateKey));
            var keys = (AsymmetricCipherKeyPair) pr.ReadObject();
            var rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters) keys.Private);
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaParams);
                return Jose.JWT.Encode(claims, rsa, JwsAlgorithm.RS512);
            }
        }

        private string sha256(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            var hasher = new SHA256Managed();
            var hashed = hasher.ComputeHash(bytes);
            var rv = string.Empty;
            foreach (var b in hashed)
            {
                rv += $"{b:x2}";
            }

            return rv;
        }
    }
}