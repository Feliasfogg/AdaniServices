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
using System.ServiceModel.Channels;
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
      public void GetTcpSettingsTest() {
         var udpHelper = new CommandListener(4444, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000));
         udpHelper.ListenUdpAsync();

         var sender = new CommandSender("192.168.1.255", 4444);
         sender.GetTcpSettings();
      }

      [TestMethod]
      public async Task AuthorizationTest() {
         var sender = new CommandSender("192.168.1.255", 4444);
         sender.GetTcpSettings();

         var command = new AuthorizationCommand() {
            Command = CommandActions.Authorization,
            Login = "felias",
            Password = "fenris"
         };

         var serializer = new XmlSerialization<AuthorizationCommand>();
         Stream stream = serializer.Serialize(command);
         byte[] btarr = new byte[stream.Length];
         stream.Read(btarr, 0, btarr.Length);
         string commandString = Encoding.ASCII.GetString(btarr);

         sender.SendCommand(commandString);
         byte[] btarrResponse = await sender.ReceiveData();
         string strResponse = Encoding.ASCII.GetString(btarrResponse);
      }

      [TestMethod]
      public async Task GetAuthInfoTest() {
         var sender = new CommandSender("192.168.1.255", 4444);
         sender.GetTcpSettings();

         var command = new AuthorizationCommand() {
            Command = CommandActions.Authorization,
            Login = "felias",
            Password = "fenris"
         };

         var serializer1 = new XmlSerialization<AuthorizationCommand>();
         Stream stream = serializer1.Serialize(command);
         byte[] btarr = new byte[stream.Length];
         stream.Read(btarr, 0, btarr.Length);
         string commandString = Encoding.ASCII.GetString(btarr);

         sender.SendCommand(commandString);

         byte[] btarrResponse = await sender.ReceiveData();
         string strResponse = Encoding.ASCII.GetString(btarrResponse);


         var authInfoCommand = new ServiceCommand() {
            Command = CommandActions.AuthorizationInfo,
            SessionKey = strResponse,
         };

         var serializer2 = new XmlSerialization<ServiceCommand>();
         stream = serializer2.Serialize(authInfoCommand);
         btarr = new byte[stream.Length];
         stream.Read(btarr, 0, btarr.Length);
         string strAuthInfoCommand = Encoding.ASCII.GetString(btarr);


         sender.SendCommand(strAuthInfoCommand);
         btarrResponse = await sender.ReceiveData();
         string strAuthInfoResult = Encoding.ASCII.GetString(btarrResponse);
      }
   }
}