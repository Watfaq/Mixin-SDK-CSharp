#region

using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;

#endregion

namespace Mixin.Messenger
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var clientId = "4f11c777-3d31-468b-a099-5577438112ef";
            var clientSecret = "9316a61a51f27b6be744c39d348490b1f5cd56ab8ab9b939b3c4f806d296dbd7";
            var pinCode = "491169";
            var sessionId = "bda2c4ad-f1d3-4c56-998e-ac4e31af2d62";

            var pinToken =
                "hMr9RJqbYWXgYGrv0h8xzDJg1kQ1JRqrm+dwIyWrvJ8TQUsbR5iwS+iIc4g1H9G1eKwndi6D4SIddnOJJaI+EoX0GglT347gRBNNIckOfQnEA6BuOCoDIBCrBSWbXrUetaj5K6wtUrhN6m9ZvOolJiXlJZOmTAvs3AiLTDogBGU=";
            var privateKey = @"-----BEGIN RSA PRIVATE KEY-----
MIICXQIBAAKBgQCgJGu8uYH6p63W0WQxt7mxc8U3Sn9oyetp+gpd1FDkoCRR/kO2
bwai6RVtR+76HubanG2yHoigVqd5reIyOqn5+Ws71tEoTLv//2UH0j+1QSROryPk
8GwVoix03UImm8PN6ulebpquxdpuiN/eQDd+veVMeVkWFxDiZ9FaQb6UzwIDAQAB
AoGBAJhcDmSv2gowHiSTc1AaDIHYM3o3VqDL+z4ItnQu3AeolOWtk56uYxH70Hb4
SVWOsPSsf3FHu5VQ3GXYGazQQ4YWXY1SewL4BTQ15dYaxuxBDVQUfh08Tncega3x
Dhbvm0GZhur0azBPPKepMU8Uqviwoxe7ZxNm9X0mn1OutdiBAkEA+9BDJo6n2eLC
VNPIpY6uGS2gHRFbJsCAUo9OqOASWmdN4oQk3UNrkJngiU7KKMpga/l0zczbxasH
RDsJbcTrJQJBAKLN/6bU5+Z/dLxk6+AKqwYX1cfuNCYdrOgdELZWWmxhWznTgKpH
R7KreOMmsJwlycsCE9mJaHEGUthRsQIu1+MCQB0TI9281PacbxG6Tk2HVuTDVtxk
V6D20xo1P8DR9myHxv9jgJonobDVg453EXJ6Q33UFL96atm88J7ZLjKFbR0CQDXE
asYiUmQhe+AsRGo9m7XrcUMSPE7KRixyTO6rHjnk/UffvgJ+gANV9hqu0G0BXd+Z
1AxoAQIy4HFoqVjbN5kCQQCE732gDMW85Y0PSrMetwYOQFKY5bWUNqEJ6iQTFGay
2gmfqy4hCcbZAJL0vEROJ6XoUjQFQoUigmSRiKvWwpRk
-----END RSA PRIVATE KEY-----";

            var CNB_ASSET_ID = "965e5c6e-434c-3fa9-b780-c50f43cd955c";


            var ws = new WebSocketClient(clientId, sessionId, privateKey)
            {
                onCloseDelegate = (sender, eventArgs) => { Console.WriteLine("closed " + eventArgs.ToString()); },
                onErrorDelegate = (sender, eventArgs) => { Console.WriteLine("Error " + eventArgs.ToString()); }
            };
            ws.onMessageDelegate = (sender, eventArgs, message) =>
            {
                var data = JsonConvert.DeserializeObject<Message>(message);

                if (!string.IsNullOrEmpty(data.Error))
                {
                    Console.WriteLine($"Error occured: {data.Error}");
                    return;
                }

                string action = data.Action;

                var supportedOperations = new List<string>
                {
                    "ACKNOWLEDGE_MESSAGE_RECEIPT", "CREATE_MESSAGE", "LIST_PENDING_MESSAGES"
                };
                if (!supportedOperations.Contains(action))
                {
                    Console.WriteLine("Unknown Action");
                    return;
                }

                if (action == "ACKNOWLEDGE_MESSAGE_RECEIPT")
                {
                    return;
                }

                if (action == "CREATE_MESSAGE")
                {
                    var payload = data.Data;
                    var messageId = payload.MessageId;
                    var messageType = payload.Type;
                    var messageCategory = payload.Category;
                    var userId = payload.UserId;
                    var conversationId = payload.ConversationId;
                    var payloadData = payload.DataData;


                    string messageContent = ws.Base64Decode(payloadData);
                    ws.AckMessage(messageId);


                    if (messageCategory == "PLAIN_TEXT" && messageType == "message")
                    {
                        if (messageContent == "1")
                        {
                            ws.SendUserContactCard(conversationId, userId, clientId);
                        }
                        else if (messageContent == "2")
                        {
                            ws.SendUserAppButton(conversationId, userId, "https://google.com", "打开链接");
                        }
                        else if (messageContent == "3")
                        {
                            ws.SendUserPayAppButton(conversationId, userId, "打钱", CNB_ASSET_ID, "100");
                        }
                        else
                        {
                            ws.SendUserPlainText(conversationId, userId, "尝试发送数字 1 或者 2");
                        }
                    }
                }
            };
            ws.Run();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}