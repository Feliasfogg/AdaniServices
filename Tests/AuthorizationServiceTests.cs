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

         var serializer = new XmlSerializer<TestClass>();
         Stream stream = serializer.Serialize(testInstance);
         TestClass deserializeInstance = serializer.Deserialize(stream);

         Assert.IsTrue(testInstance.Name == deserializeInstance.Name);
         Assert.IsTrue(testInstance.Value == deserializeInstance.Value);
      }

      [TestMethod]
      public void GetTcpSettingsTest() {
         var sender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4444);
         sender.GetTcpSettings();
      }

      [TestMethod]
      public async Task AuthorizationTest() {
         var sender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4444);
         sender.GetTcpSettings();

         var command = new AuthorizationCommand() {
            Command = CommandActions.Authorization,
            Login = "felias",
            Password = "fenris"
         };

         var serializer = new XmlSerializer<AuthorizationCommand>();
         Stream stream = serializer.Serialize(command);
         byte[] btarr = new byte[stream.Length];
         stream.Read(btarr, 0, btarr.Length);
         string commandString = Encoding.ASCII.GetString(btarr);

         sender.SendCommand(commandString);
         byte[] btarrResponse = await sender.ReceiveDataAsync();
         //сессионный ключ
         string strResponse = Encoding.ASCII.GetString(btarrResponse);
      }

      [TestMethod]
      public async Task GetAuthInfoTest() {
         var sender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4444);
         sender.GetTcpSettings();
         //command for authorization
         var command = new AuthorizationCommand() {
            Command = CommandActions.Authorization,
            Login = "felias",
            Password = "fenris"
         };
         //authorization and update session key
         var serializer1 = new XmlSerializer<AuthorizationCommand>();
         string commandString = serializer1.SerializeToXmlString(command);
         sender.SendCommand(commandString);

         byte[] btarrResponse = await sender.ReceiveDataAsync();
         string strSessionKey = Encoding.ASCII.GetString(btarrResponse); //sessionkey

         //compare authorization status
         var authInfoCommand = new ServiceCommand() {
            Command = CommandActions.AuthorizationInfo,
            SessionKey = strSessionKey,
         };

         var serializer2 = new XmlSerializer<ServiceCommand>();
         string strAuthInfoCommand = serializer2.SerializeToXmlString(authInfoCommand);


         sender.SendCommand(strAuthInfoCommand);
         btarrResponse = await sender.ReceiveDataAsync();
         string strAuthInfoResult = Encoding.ASCII.GetString(btarrResponse); //get user information
         Assert.IsTrue(strAuthInfoResult != String.Empty);
      }

      [TestMethod]
      public async Task EditUserTest() {
         var sender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4444);
         sender.GetTcpSettings();

         var command = new AuthorizationCommand() {
            Command = CommandActions.Authorization,
            Login = "felias",
            Password = "fenris"
         };

         var serializer1 = new XmlSerializer<AuthorizationCommand>();
         string commandString = serializer1.SerializeToXmlString(command);
         sender.SendCommand(commandString);

         byte[] btarrResponse = await sender.ReceiveDataAsync();
         string strSessionKey = Encoding.ASCII.GetString(btarrResponse);

         //команда получения инфы об авторизации
         var authInfoCommand = new ServiceCommand() {
            Command = CommandActions.AuthorizationInfo,
            SessionKey = strSessionKey,
         };

         var serializer2 = new XmlSerializer<ServiceCommand>();
         string strAuthInfoCommand = serializer2.SerializeToXmlString(authInfoCommand);

         sender.SendCommand(strAuthInfoCommand);
         btarrResponse = await sender.ReceiveDataAsync();
         string strUserInfo = Encoding.ASCII.GetString(btarrResponse); //get user information

         var serializer3 = new XmlSerializer<UserInfo>();
         UserInfo user = serializer3.Deserialize(strUserInfo);
         //edit userinfo
         user.Login = "newlogin";
         user.Password = "newpasword";

         //command for edit user info
         var editCommand = new EditUserCommand() {
            Command = CommandActions.EditUser,
            Info = user
         };
         var serizalier4 = new XmlSerializer<EditUserCommand>();
         string strEditCommand = serizalier4.SerializeToXmlString(editCommand);
         sender.SendCommand(strEditCommand);
         btarrResponse = await sender.ReceiveDataAsync();
         string strResponse = Encoding.ASCII.GetString(btarrResponse);

         Assert.IsTrue(strResponse == "ok");
      }
   }
}