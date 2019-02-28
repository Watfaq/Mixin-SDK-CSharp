using System;
using System.Linq;
using Mixin.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Mixin.Test
{
    [TestClass]
    public class NetworkTests
    {
        private readonly MixinClient mixinClient;

        public NetworkTests()
        {
            var clientId = "4f11c777-3d31-468b-a099-5577438112ef";
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

            mixinClient = new MixinClient(clientId, sessionId, pinToken, pinCode, privateKey);
        }

//        [TestMethod]
        public void TestCreatePin()
        {
            var testPin = "123456";
            var newPin = mixinClient.CreatePin("491169", testPin);
            var fullName = newPin.FullName;
            var changeBack = mixinClient.CreatePin(testPin, "491169");
            Assert.AreEqual(fullName, changeBack.FullName);
        }

        [TestMethod]
        public void TestSearchAsserts()
        {
            var assets = mixinClient.SearchAssets("eos");
            Assert.IsTrue(assets.Count > 0);
            Assert.IsTrue(assets.First().Symbol.Equals("EOS"));
        }

        [TestMethod]
        public void TestTopAssets()
        {
            var assets = mixinClient.TopAssets();
            Assert.IsTrue(assets.Count > 0);

            Assert.IsNotNull(assets.First().Name);
        }

//        [TestMethod]
        public void TestTransfer()
        {
            var toClientId = "ea02ebc3-9be3-4a35-985b-6587fd28f493";
            var traceId = Guid.NewGuid().ToString();
            var networkTraceId = mixinClient.Transfer(toClientId, "0.0001", Assets.CNB, "Test Transfer", traceId)
                .TraceId;
            Assert.AreEqual(traceId, networkTraceId.ToString());

            var transfer = mixinClient.GetTransfer(traceId);
            Assert.AreEqual("Test Transfer", transfer.Memo);
        }

 //       [TestMethod]
        public void TestVerifyPin()
        {
            var result = mixinClient.VerifyPin("491169");
            Assert.IsTrue(result.HasPin);
        }
    }
}