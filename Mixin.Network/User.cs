#region

using System.Collections.Generic;
using Newtonsoft.Json;

#endregion

namespace Mixin.Network
{
    public partial class User
    {
        private readonly string accessToken;
        private readonly string baseUrl = "https://api.mixin.one";

        private readonly string clientId;
        private readonly string pinCode;
        private readonly string pinToken;
        private readonly string privateKey;
        private readonly string sessionId;

        public User(string clientId = "", string sessionId = "", string pinToken = "", string pinCode = "",
            string privateKey = "", string accessToken = "")
        {
            this.clientId = clientId;
            this.sessionId = sessionId;
            this.pinToken = pinToken;
            this.pinCode = pinCode;
            this.privateKey = privateKey;
            this.accessToken = accessToken;
        }

        public string GetMyAssets()
        {
            return sendGetRequest("/assets", accessToken);
        }

        public string UpdateMyPreference(string receiveMessageSource = "EVERYBODY",
            string acceptConversationSource = "EVERYBODY")
        {
            return sendPostRequest("/me/preferences", JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                {"receive_message_source", receiveMessageSource},
                {"accept_conversation_source", acceptConversationSource}
            }), accessToken);
        }

        public string UpdateMyProfile(string fullName, string avatarBase64 = "")
        {
            return sendPostRequest("/me", JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                {"full_name", fullName},
                {"avatar_base64", avatarBase64}
            }), accessToken);
        }

        public string GetUsersInfo(string userIds)
        {
            return sendPostRequest("/users/fetch", userIds, accessToken);
        }

        public string GetUserInfo(string userId)
        {
            return sendGetRequest($"/users/{userId}", accessToken);
        }

        public string SearchUser(string q)
        {
            return sendGetRequest($"/search/{q}", accessToken);
        }

        public string RotateUserQR()
        {
            return sendGetRequest("/me/code", accessToken);
        }

        public string GetMyFriends()
        {
            return sendGetRequest("/friends", accessToken);
        }

        public string CreateConversation(string category, string conversationId, string participants, string action,
            string role, string userId)
        {
            return sendPostRequest("/conversations", JsonConvert.SerializeObject(new Dictionary<string, string>
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
            return sendGetRequest($"/conversations/{conversationId}");
        }
    }
}