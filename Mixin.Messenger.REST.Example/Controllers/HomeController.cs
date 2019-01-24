using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mixin.Network;
using Newtonsoft.Json;

namespace Mixin.Messenger.REST.Example.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private string clientId = "4f11c777-3d31-468b-a099-5577438112ef";
        private string clientSecret = "9316a61a51f27b6be744c39d348490b1f5cd56ab8ab9b939b3c4f806d296dbd7";

        public ActionResult Index()
        {
            var scope = "PROFILE:READ+PHONE:READ+CONTACTS:READ+ASSETS:READ";
            var authUrl = $"https://mixin.one/oauth/authorize?client_id={clientId}&scope={scope}&response_type=code";
            return Redirect(authUrl);
        }

        [Route("callback")]
        public async Task<string> Callback(string code)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var getTokenUrl = "https://api.mixin.one/oauth/token";
            using (var httpClient = new HttpClient())
            {
                var data = new Dictionary<string, string>
                {
                    {"client_id", clientId},
                    {"client_secret", clientSecret},
                    {"code", code}
                };
                var req = await httpClient.PostAsync(getTokenUrl,
                    new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
                dynamic res = JsonConvert.DeserializeObject(await req.Content.ReadAsStringAsync());
                string accessToken = res.data.access_token;
                var mixin = new User(accessToken: accessToken);
                return mixin.GetMyFriends();
            }
        }
    }
}