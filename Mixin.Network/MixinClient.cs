#region

using System.Collections.Generic;
using Newtonsoft.Json;

#endregion

namespace Mixin.Network
{
    public partial class MixinClient
    {

        private readonly string pinCode;
        private MixinClientTransport transport;

        public MixinClient(string clientId = "", string sessionId = "", string pinToken = "", string pinCode = "",
            string privateKey = "")
        {
            this.pinCode = pinCode;
            transport = new MixinClientTransport(clientId, sessionId, pinToken, privateKey);
        }
    }
}