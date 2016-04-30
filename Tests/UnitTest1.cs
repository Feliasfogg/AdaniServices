using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands;
using CoreLib.Helpers;
using CoreLib.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.ServiceReference;
using System.ServiceModel.Channels;

namespace Tests {
   [TestClass]
   public class UnitTest1 {
      [TestMethod]
      public void SerializeDeserializeTest() {
         var testInstance = new TestClass() {
            Name = "Test",
            Value = 1
         };

         var serializer = new XmlSerialization<TestClass>();
         Stream stream = serializer.Serialize(testInstance);
         TestClass deserializeInstance = serializer.Deserialize(stream);

         Assert.IsTrue(testInstance.Name == deserializeInstance.Name);
         Assert.IsTrue(testInstance.Value == deserializeInstance.Value);
      }

      [TestMethod]
      public async Task UdpTest() {
         var udpHelper = new UdpListener(4444, "net.tcp://127.0.0.1:11000");
         await udpHelper.ListenAsync();
      }

      [TestMethod]
      public void SendLogin() {
         var udpHelper = new UdpSender("192.168.1.255", 4444);
         string strTcpEndPoint = udpHelper.RequireTcpSettings();
         var remoteTcpEndPoint = new EndpointAddress(strTcpEndPoint);

         var data = new AuthorizationCommand() {
            Login = "felias",
            PasswordHash = "qe4wtdsfghdfggf"
         };
         var k = new AuthorizationClient(new NetTcpBinding(SecurityMode.Transport), remoteTcpEndPoint);
         string key = k.GetSessionKey(data);
      }
      [TestMethod]
      public void SendCommandOnUdp() {
         var command = new ServiceCommand() {
            Command = CommandActions.GetUserInfo,
            SessionKey = "werdwfsdfaasfd"
         };
         var udpHelper = new UdpSender("192.168.1.255", 4444);
         udpHelper.SendCommand(command);

      }
   }
}