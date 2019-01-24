#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Jose;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

#endregion

namespace Mixin.Network
{
    public partial class User
    {
        private string signAuthToken(string method, string uri, string body = "")
        {
            var iat = DateTimeOffset.UtcNow;
            var exp = iat + TimeSpan.FromDays(30);
            var sig = sha256(method, uri, body);
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
                return JWT.Encode(claims, rsa, JwsAlgorithm.RS512);
            }
        }

        private string encryptPin(string pin, UInt64 iterator)
        {
            var pinTokenBytes = Convert.FromBase64String(pinToken);
            var pr = new PemReader(new StringReader(privateKey));
            var keys = (AsymmetricCipherKeyPair) pr.ReadObject();

            var eng = new OaepEncoding(new RsaEngine(), new Sha256Digest(), new Sha256Digest(),
                Encoding.UTF8.GetBytes(sessionId));
            eng.Init(false, keys.Private);

            var plainTextBytes = new List<byte>();
            var blockSize = eng.GetInputBlockSize();
            for (var chunkPosition = 0; chunkPosition < pinTokenBytes.Length; chunkPosition += blockSize)
            {
                var chunkSize = Math.Min(blockSize, pinTokenBytes.Length - chunkPosition);
                plainTextBytes.AddRange(eng.ProcessBlock(pinTokenBytes, chunkPosition, chunkSize));
            }

            var keyBytes = plainTextBytes.ToArray();


            var pinBytes = new List<byte>(Encoding.ASCII.GetBytes(pin));
            var timeBytes = BitConverter.GetBytes(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            pinBytes.AddRange(timeBytes);
            var iteratorBytes = BitConverter.GetBytes(iterator);
            pinBytes.AddRange(iteratorBytes);

            var aesBlockSize = 16;
            var padding = aesBlockSize - pinBytes.Count % aesBlockSize;
            var padText = Enumerable.Repeat(Convert.ToByte(padding), padding);
            pinBytes.AddRange(padText);

            using (var ms = new MemoryStream())
            {
                using (var cryptor = new AesManaged())
                {
                    cryptor.Mode = CipherMode.CBC;
                    cryptor.Padding = PaddingMode.None;

                    byte[] iv = cryptor.IV;

                    using (var cs = new CryptoStream(ms, cryptor.CreateEncryptor(keyBytes, iv), CryptoStreamMode.Write))
                    {
                        cs.Write(pinBytes.ToArray(), 0, pinBytes.Count);
                    }

                    var encrypted = ms.ToArray();
                    var result = new byte[iv.Length + encrypted.Length];
                    Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                    Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);
                    return Convert.ToBase64String(result);
                }
            }
        }

        private string sha256(string method, string uri, string body)
        {
            var bytes = Encoding.UTF8.GetBytes(method + uri + body);
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