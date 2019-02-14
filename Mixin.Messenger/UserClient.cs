using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Mixin.Network;
using Newtonsoft.Json;

namespace Mixin.Messenger
{
    public class UserClient
    {
        private string accessToken;
        private MixinClientTransport transport;

        public UserClient(string accessToken)
        {
            this.accessToken = accessToken;
            transport = new MixinClientTransport();

        }
        public string GetMyAssets()
        {
            return transport.SendGetRequest("/assets", accessToken);
        }

        public string UpdateMyPreference(string receiveMessageSource = "EVERYBODY",
            string acceptConversationSource = "EVERYBODY")
        {
            return transport.SendPostRequest("/me/preferences", JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                {"receive_message_source", receiveMessageSource},
                {"accept_conversation_source", acceptConversationSource}
            }), accessToken);
        }

        public string ReadProfile()
        {
            return sendGetRequest("/me", accessToken);
        }

        public string UpdateMyProfile(string fullName, string avatarBase64 = "")
        {
            return transport.SendPostRequest("/me", JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                {"full_name", fullName},
                {"avatar_base64", avatarBase64}
            }), accessToken);
        }

        public string GetUsersInfo(string userIds)
        {
            return transport.SendPostRequest("/users/fetch", userIds, accessToken);
        }

        public string GetUserInfo(string userId)
        {
            return transport.SendPostRequest($"/users/{userId}", accessToken);
        }

        public string SearchUser(string q)
        {
            return transport.SendPostRequest($"/search/{q}", accessToken);
        }

        public string RotateUserQR()
        {
            return transport.SendPostRequest("/me/code", accessToken);
        }

        public string GetMyFriends()
        {
            return transport.SendPostRequest("/friends", accessToken);
        }

        public string CreateConversation(string category, string conversationId, string participants, string action,
            string role, string userId)
        {
            return transport.SendPostRequest("/conversations", JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                {"category", category},
                {"conversation_id", conversationId},
                {"participants", participants},
                {"action", action},
                {"role", role},
                {"user_id", userId}
            }), accessToken);
        }

        public string GetConversation(string conversationId)
        {
            return transport.SendGetRequest($"/conversations/{conversationId}");
        }
    }
}
