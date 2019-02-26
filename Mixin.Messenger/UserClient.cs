using System.Collections.Generic;
using Mixin.Messenger.Models;
using Mixin.Network;
using Newtonsoft.Json;

namespace Mixin.Messenger
{
    public class UserClient
    {
        private readonly string accessToken;
        private readonly MixinClientTransport transport;

        public UserClient(string accessToken)
        {
            this.accessToken = accessToken;
            transport = new MixinClientTransport();
        }

        public UserProfileData ReadProfile()
        {
            return JsonConvert.DeserializeObject<UserProfileModel>(transport.SendGetRequest("/me", accessToken)).Data;
        }

        public UserProfileData UpdateMyPreference(string receiveMessageSource = "EVERYBODY",
            string acceptConversationSource = "EVERYBODY")
        {
            var resp = transport.SendPostRequest("/me/preferences", JsonConvert.SerializeObject(
                new Dictionary<string, string>
                {
                    {"receive_message_source", receiveMessageSource},
                    {"accept_conversation_source", acceptConversationSource}
                }), accessToken);
            return JsonConvert.DeserializeObject<UserProfileModel>(resp).Data;
        }

        public UserProfileData UpdateMyProfile(string fullName, string avatarBase64 = "")
        {
            var resp = transport.SendPostRequest("/me", JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                {"full_name", fullName},
                {"avatar_base64", avatarBase64}
            }), accessToken);
            return JsonConvert.DeserializeObject<UserProfileModel>(resp).Data;
        }

        public List<UserProfileData> GetUsersInfo(string userIds)
        {
            return JsonConvert
                .DeserializeObject<ProfileListModel>(transport.SendPostRequest("/users/fetch", userIds, accessToken))
                .Data;
        }

        public UserProfileData GetUserInfo(string userId)
        {
            return JsonConvert
                .DeserializeObject<UserProfileModel>(transport.SendPostRequest($"/users/{userId}", "", accessToken))
                .Data;
        }

        public UserProfileData SearchUser(string q)
        {
            return JsonConvert
                .DeserializeObject<UserProfileModel>(transport.SendPostRequest($"/search/{q}", "", accessToken)).Data;
        }

        public UserProfileData RotateUserQR()
        {
            return JsonConvert
                .DeserializeObject<UserProfileModel>(transport.SendPostRequest("/me/code", "", accessToken)).Data;
        }

        public List<UserProfileData> GetMyFriends()
        {
            return JsonConvert
                .DeserializeObject<ProfileListModel>(transport.SendPostRequest("/friends", "", accessToken)).Data;
        }

        public AttachmentData CreateAttachment()
        {
            return JsonConvert
                .DeserializeObject<AttachmentModel>(transport.SendPostRequest("/attachments", "", accessToken))
                .AttachmentData;
        }

        public ConversationData CreateConversation(string category, string conversationId, string participants,
            string action,
            string role, string userId)
        {
            var resp = transport.SendPostRequest("/conversations", JsonConvert.SerializeObject(
                new Dictionary<string, string>
                {
                    {"category", category},
                    {"conversation_id", conversationId},
                    {"participants", participants},
                    {"action", action},
                    {"role", role},
                    {"user_id", userId}
                }), accessToken);

            return JsonConvert.DeserializeObject<ConversationModel>(resp).Data;
        }

        public ConversationData GetConversation(string conversationId)
        {
            return JsonConvert
                .DeserializeObject<ConversationModel>(transport.SendGetRequest($"/conversations/{conversationId}",
                    accessToken)).Data;
        }
    }
}