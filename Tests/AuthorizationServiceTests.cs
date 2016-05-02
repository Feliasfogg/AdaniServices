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
using AuthorizationWcfLib.Data;
using CoreLib.Entity;

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
         var udpHelper = new UdpCommandListener(4444, "net.tcp://127.0.0.1:11000");
         await udpHelper.ListenAsync();
      }

      [TestMethod]
      public void AuthorizationTest() {
         var sender = new UdpCommandSender("192.168.1.255", 4444);
         string strTcpEndPoint = sender.GetTcpSettings();
         var remoteTcpEndPoint = new EndpointAddress(strTcpEndPoint);

         var data = new AuthorizationCommand() {
            Login = "felias",
            Password = "fenris"
         };
         var client = new AuthorizationClient(new NetTcpBinding(SecurityMode.Transport), remoteTcpEndPoint);
         string sessionKey = client.Authorization(data);
         Assert.IsTrue(sessionKey != null);
      }

      [TestMethod]
      public void GetAuthorizationInfoTest() {
         var sender = new UdpCommandSender("192.168.1.255", 4444);
         string strTcpEndPoint = sender.GetTcpSettings();
         var remoteTcpEndPoint = new EndpointAddress(strTcpEndPoint);

         var data = new AuthorizationCommand() {
            Login = "felias",
            Password = "fenris",
         };
         var client = new AuthorizationClient(new NetTcpBinding(SecurityMode.Transport), remoteTcpEndPoint);
         string sessionKey = client.Authorization(data);
         Assert.IsTrue(sessionKey != String.Empty);


         var command = new ServiceCommand() {
            Command = CommandActions.AuthorizationInfo,
            SessionKey = sessionKey
         };

         UserEntity userEntity = sender.GetUserInfo(command);
      }
   }
}