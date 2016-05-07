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
using System.Xml.Serialization;
using CoreLib.Commands.Authorization;
using CoreLib.Commands.Common;
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
      public void AddDeleteUserTest() {
            var sender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4444);
            sender.GetTcpSettings();
            //создание юзера
            var newUser = new UserInfo() {
               Login = "felias",
               Password = "fenris",
               Name = "pavel",
               AccessLevel = 18,
            };
            //команда создания юзера
            var addUserCommand = new UserCommand() {
               Command = CommandActions.AddUser,
               Info = newUser
            };
            //сериализация
            var serializer = new XmlSerializer<UserCommand>();
            string strXml = serializer.SerializeToXmlString(addUserCommand);
            //отрпавка команды
            sender.SendCommand(strXml);
            byte[] bytes = sender.ReceiveData(); //получение ответа //БАГ - пытаемься получить данные еще до того как они будут отправлены сервером
            Assert.IsTrue(Encoding.ASCII.GetString(bytes) == "ok");

            //команда авторизации
            var authCommand = new AuthorizationCommand() {
               Command = CommandActions.Authorization,
               Login = "felias",
               Password = "fenris"
            };

            var serializer1 = new XmlSerializer<AuthorizationCommand>();
            string commandString = serializer1.SerializeToXmlString(authCommand);
            sender.SendCommand(commandString);
            //отрпавка команды на авторизацию, в ответ от сервера должен прийти сессионный ключ авторизации
            bytes = sender.ReceiveData();
            string strSessionKey = Encoding.ASCII.GetString(bytes);
            Assert.IsTrue(strSessionKey != "error" && strSessionKey != String.Empty);

            //команда получения инфы о пользователе
            var authInfoCommand = new ServiceCommand() {
               Command = CommandActions.GetUserInfo,
               SessionKey = strSessionKey,
            };

            var serializer2 = new XmlSerializer<ServiceCommand>();
            string strAuthInfoCommand = serializer2.SerializeToXmlString(authInfoCommand);

            sender.SendCommand(strAuthInfoCommand);
            bytes = sender.ReceiveData();
            string strUserInfo = Encoding.ASCII.GetString(bytes); //инфа о пользователе
            Assert.IsTrue(strUserInfo != "error" && strUserInfo != String.Empty);

            var serializer3 = new XmlSerializer<UserInfo>();
            UserInfo user = serializer3.Deserialize(strUserInfo); //десериализация строки инфы о пользователе в объект

            //удаление пользователя
            var deleteCommand = new DeleteUserCommand() {
               Command = CommandActions.DeleteUser,
               UserId = user.Id
            };

            var serializer4 = new XmlSerializer<DeleteUserCommand>();
            string strDeleteCommand = serializer4.SerializeToXmlString(deleteCommand);
            sender.SendCommand(strDeleteCommand);
            bytes = sender.ReceiveData();
            Assert.IsTrue(Encoding.ASCII.GetString(bytes) == "ok");
      }


      [TestMethod]
      public void AuthorizationTest() {
         var sender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4444);
         sender.GetTcpSettings();
         //создание юзера
         var newUser = new UserInfo() {
            Login = "felias",
            Password = "fenris",
            Name = "pavel",
            AccessLevel = 18,
         };
         //команда создания юзера
         var addUserCommand = new UserCommand() {
            Command = CommandActions.AddUser,
            Info = newUser
         };
         //сериализация
         var serializer = new XmlSerializer<UserCommand>();
         string strXml = serializer.SerializeToXmlString(addUserCommand);
         //отрпавка команды
         sender.SendCommand(strXml);
         byte[] bytes = sender.ReceiveData(); //получение ответа //БАГ - пытаемься получить данные еще до того как они будут отправлены сервером
         Assert.IsTrue(Encoding.ASCII.GetString(bytes) == "ok");

         //команда авторизации
         var authCommand = new AuthorizationCommand() {
            Command = CommandActions.Authorization,
            Login = "felias",
            Password = "fenris"
         };

         var serializer1 = new XmlSerializer<AuthorizationCommand>();
         string commandString = serializer1.SerializeToXmlString(authCommand);
         sender.SendCommand(commandString);
         //отрпавка команды на авторизацию, в ответ от сервера должен прийти сессионный ключ авторизации
         bytes = sender.ReceiveData();
         string strSessionKey = Encoding.ASCII.GetString(bytes);
         Assert.IsTrue(strSessionKey != "error" && strSessionKey != String.Empty);
      }

      [TestMethod]
      public void GetAuthInfoTest() {
         var sender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4444);
         sender.GetTcpSettings();
         //создание юзера
         var newUser = new UserInfo() {
            Login = "felias",
            Password = "fenris",
            Name = "pavel",
            AccessLevel = 18,
         };
         //команда создания юзера
         var addUserCommand = new UserCommand() {
            Command = CommandActions.AddUser,
            Info = newUser
         };
         //сериализация
         var serializer = new XmlSerializer<UserCommand>();
         string strXml = serializer.SerializeToXmlString(addUserCommand);
         //отрпавка команды
         sender.SendCommand(strXml);
         byte[] bytes = sender.ReceiveData(); //получение ответа //БАГ - пытаемься получить данные еще до того как они будут отправлены сервером
         Assert.IsTrue(Encoding.ASCII.GetString(bytes) == "ok");

         //команда авторизации
         var authCommand = new AuthorizationCommand() {
            Command = CommandActions.Authorization,
            Login = "felias",
            Password = "fenris"
         };

         var serializer1 = new XmlSerializer<AuthorizationCommand>();
         string commandString = serializer1.SerializeToXmlString(authCommand);
         sender.SendCommand(commandString);
         //отрпавка команды на авторизацию, в ответ от сервера должен прийти сессионный ключ авторизации
         bytes = sender.ReceiveData();
         string strSessionKey = Encoding.ASCII.GetString(bytes);
         Assert.IsTrue(strSessionKey != "error" && strSessionKey != String.Empty);

         //команда получения инфы о пользователе
         var authInfoCommand = new ServiceCommand() {
            Command = CommandActions.GetUserInfo,
            SessionKey = strSessionKey,
         };

         var serializer2 = new XmlSerializer<ServiceCommand>();
         string strAuthInfoCommand = serializer2.SerializeToXmlString(authInfoCommand);

         sender.SendCommand(strAuthInfoCommand);
         bytes = sender.ReceiveData();
         string strUserInfo = Encoding.ASCII.GetString(bytes); //инфа о пользователе
         Assert.IsTrue(strUserInfo != "error" && strUserInfo != String.Empty);

         var serializer3 = new XmlSerializer<UserInfo>();
         UserInfo user = serializer3.Deserialize(strUserInfo); //десериализация строки инфы о пользователе в объект
         Assert.IsTrue(user.Login == authCommand.Login && user.Password == authCommand.Password);
      }

      [TestMethod]
      public void EditUserTest() {
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

         byte[] btarrResponse = sender.ReceiveData();
         string strSessionKey = Encoding.ASCII.GetString(btarrResponse);
         Assert.IsTrue(strSessionKey != "error");

         //команда получения инфы об авторизации
         var authInfoCommand = new ServiceCommand() {
            Command = CommandActions.GetUserInfo,
            SessionKey = strSessionKey,
         };

         var serializer2 = new XmlSerializer<ServiceCommand>();
         string strAuthInfoCommand = serializer2.SerializeToXmlString(authInfoCommand);

         sender.SendCommand(strAuthInfoCommand);
         btarrResponse = sender.ReceiveData();
         string strUserInfo = Encoding.ASCII.GetString(btarrResponse); //get user information

         var serializer3 = new XmlSerializer<UserInfo>();
         UserInfo user = serializer3.Deserialize(strUserInfo);
         //edit userinfo
         user.Login = "newlogin";
         user.Password = "newpasword";

         //command for edit user info
         var editCommand = new UserCommand() {
            Command = CommandActions.EditUser,
            Info = user
         };
         var serizalier4 = new XmlSerializer<UserCommand>();
         string strEditCommand = serizalier4.SerializeToXmlString(editCommand);
         sender.SendCommand(strEditCommand);
         btarrResponse = sender.ReceiveData();
         string strResponse = Encoding.ASCII.GetString(btarrResponse);

         Assert.IsTrue(strResponse == "ok");
      }
   }
}