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
   public class AuthrorizationServiceTests {
      [TestMethod]
      public void SerializeDeserializeTest() {
         var testInstance = new TestClass() {
            Name = "Test",
            Value = 1
         };

         Stream stream = XmlSerializer<TestClass>.Serialize(testInstance);
         TestClass deserializeInstance = XmlSerializer<TestClass>.Deserialize(stream);

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
         var accessBytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 255 };
         Int64 accessLevel = BitConverter.ToInt64(accessBytes, 0);

         var sender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4444);
         sender.GetTcpSettings();
         //создание юзера
         var newUser = new UserEntity() {
            Login = "felias",
            Password = "fenris",
            Name = "pavel",
            AccessLevel = accessLevel,
         };
         //команда создания юзера
         var addUserCommand = new UserCommand() {
            Command = CommandActions.AddUser,
            User = newUser
         };
         //сериализация
         string addCommand= XmlSerializer<UserCommand>.SerializeToXmlString(addUserCommand);
         //отрпавка команды
         sender.SendTcpCommand(addCommand);
         byte[] bytes = sender.ReceiveData(); //получение ответа //БАГ - пытаемься получить данные еще до того как они будут отправлены сервером
         Assert.IsTrue(Encoding.ASCII.GetString(bytes) == "ok");

         //команда авторизации
         var authCommand = new AuthorizationCommand() {
            Command = CommandActions.Authorization,
            Login = "felias",
            Password = "fenris"
         };

         string authCommandXml = XmlSerializer<AuthorizationCommand>.SerializeToXmlString(authCommand);
         sender.SendTcpCommand(authCommandXml);
         //отрпавка команды на авторизацию, в ответ от сервера должен прийти сессионный ключ авторизации
         bytes = sender.ReceiveData();
         string strSessionKey = Encoding.ASCII.GetString(bytes);

         //команда получения инфы о пользователе
         var userInfoCommand = new ServiceCommand() {
            Command = CommandActions.GetUserInfo,
            SessionKey = strSessionKey,
         };

         string userInfoCommandXml = XmlSerializer<ServiceCommand>.SerializeToXmlString(userInfoCommand);

         sender.SendTcpCommand(userInfoCommandXml);
         bytes = sender.ReceiveData();
         string strUserInfo = Encoding.ASCII.GetString(bytes); //инфа о пользователе
         Assert.IsTrue(strUserInfo != "error" && strUserInfo != String.Empty);
         UserEntity user = XmlSerializer<UserEntity>.Deserialize(strUserInfo); //десериализация строки инфы о пользователе в объект

         //удаление пользователя
         var deleteCommand = new DeleteUserCommand() {
            Command = CommandActions.DeleteUser,
            UserId = user.Id
         };

         string deleteCommandXml = XmlSerializer<DeleteUserCommand>.SerializeToXmlString(deleteCommand);
         sender.SendTcpCommand(deleteCommandXml);
         bytes = sender.ReceiveData();
         Assert.IsTrue(Encoding.ASCII.GetString(bytes) == "ok");
      }


      [TestMethod]
      public void AuthorizationTest() {
         var accessBytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 255 };
         Int64 accessLevel = BitConverter.ToInt64(accessBytes, 0);

         var sender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4444);
         sender.GetTcpSettings();
         //создание юзера
         var newUser = new UserEntity() {
            Login = "felias",
            Password = "fenris",
            Name = "pavel",
            AccessLevel = accessLevel,
         };
         //команда создания юзера
         var addUserCommand = new UserCommand() {
            Command = CommandActions.AddUser,
            User = newUser
         };
         //сериализация
         string addUserCommandXml = XmlSerializer<UserCommand>.SerializeToXmlString(addUserCommand);
         //отрпавка команды
         sender.SendTcpCommand(addUserCommandXml);
         byte[] bytes = sender.ReceiveData(); //получение ответа //БАГ - пытаемься получить данные еще до того как они будут отправлены сервером
         Assert.IsTrue(Encoding.ASCII.GetString(bytes) == "ok");

         //команда авторизации
         var authCommand = new AuthorizationCommand() {
            Command = CommandActions.Authorization,
            Login = "felias",
            Password = "fenris"
         };

         string authCommandXml = XmlSerializer<AuthorizationCommand>.SerializeToXmlString(authCommand);
         sender.SendTcpCommand(authCommandXml);
         //отрпавка команды на авторизацию, в ответ от сервера должен прийти сессионный ключ авторизации
         bytes = sender.ReceiveData();
         string strSessionKey = Encoding.ASCII.GetString(bytes);
      }

      [TestMethod]
      public void GetAuthInfoTest() {
         var accessBytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 255 };
         Int64 accessLevel = BitConverter.ToInt64(accessBytes, 0);

         var sender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4444);
         sender.GetTcpSettings();
         //создание юзера
         var newUser = new UserEntity() {
            Login = "felias",
            Password = "fenris",
            Name = "pavel",
            AccessLevel = accessLevel,
         };
         //команда создания юзера
         var addUserCommand = new UserCommand() {
            Command = CommandActions.AddUser,
            User = newUser
         };
         //сериализация
         string addUserCommandXml = XmlSerializer<UserCommand>.SerializeToXmlString(addUserCommand);
         //отрпавка команды
         sender.SendTcpCommand(addUserCommandXml);
         byte[] bytes = sender.ReceiveData(); //получение ответа //БАГ - пытаемься получить данные еще до того как они будут отправлены сервером
         Assert.IsTrue(Encoding.ASCII.GetString(bytes) == "ok");

         //команда авторизации
         var authCommand = new AuthorizationCommand() {
            Command = CommandActions.Authorization,
            Login = "felias",
            Password = "fenris"
         };

         string authCommandXml = XmlSerializer<AuthorizationCommand>.SerializeToXmlString(authCommand);
         sender.SendTcpCommand(authCommandXml);
         //отрпавка команды на авторизацию, в ответ от сервера должен прийти сессионный ключ авторизации
         bytes = sender.ReceiveData();
         string strSessionKey = Encoding.ASCII.GetString(bytes);

         //команда получения инфы о пользователе
         var userInfoCommand = new ServiceCommand() {
            Command = CommandActions.GetUserInfo,
            SessionKey = strSessionKey,
         };

         string userInfoCommandXml = XmlSerializer<ServiceCommand>.SerializeToXmlString(userInfoCommand);

         sender.SendTcpCommand(userInfoCommandXml);
         bytes = sender.ReceiveData();
         string userInfoXml = Encoding.ASCII.GetString(bytes); //инфа о пользователе

         UserEntity user = XmlSerializer<UserEntity>.Deserialize(userInfoXml); //десериализация строки инфы о пользователе в объект
         Assert.IsTrue(user.Login == authCommand.Login && user.Password == authCommand.Password);
      }

      [TestMethod]
      public void EditUserTest() {
         var accessBytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 255 };
         Int64 accessLevel = BitConverter.ToInt64(accessBytes, 0);

         var sender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4444);
         sender.GetTcpSettings();
         //создание юзера
         var newUser = new UserEntity() {
            Login = "felias",
            Password = "fenris",
            Name = "pavel",
            AccessLevel = accessLevel,
         };
         //команда создания юзера
         var addUserCommand = new UserCommand() {
            Command = CommandActions.AddUser,
            User = newUser
         };
         //сериализация
         string addUserCommandXml = XmlSerializer<UserCommand>.SerializeToXmlString(addUserCommand);
         //отрпавка команды
         sender.SendTcpCommand(addUserCommandXml);
         byte[] bytes = sender.ReceiveData();
         Assert.IsTrue(Encoding.ASCII.GetString(bytes) == "ok");

         var command = new AuthorizationCommand() {
            Command = CommandActions.Authorization,
            Login = "felias",
            Password = "fenris"
         };

         string commandString = XmlSerializer<AuthorizationCommand>.SerializeToXmlString(command);
         sender.SendTcpCommand(commandString);

         byte[] btarrResponse = sender.ReceiveData();
         string strSessionKey = Encoding.ASCII.GetString(btarrResponse);

         //команда получения инфы об авторизации
         var userInfoCommand = new ServiceCommand() {
            Command = CommandActions.GetUserInfo,
            SessionKey = strSessionKey,
         };

         string userInfoCommandXml = XmlSerializer<ServiceCommand>.SerializeToXmlString(userInfoCommand);

         sender.SendTcpCommand(userInfoCommandXml);
         btarrResponse = sender.ReceiveData();
         string strUserInfo = Encoding.ASCII.GetString(btarrResponse); //get user information

         UserEntity user = XmlSerializer<UserEntity>.Deserialize(strUserInfo);
         //edit userinfo
         user.Login = "newlogin";
         user.Password = "newpasword";

         //command for edit user info
         var editCommand = new UserCommand() {
            Command = CommandActions.EditUser,
            User = user
         };
         string editCommandXml = XmlSerializer<UserCommand>.SerializeToXmlString(editCommand);
         sender.SendTcpCommand(editCommandXml);
         btarrResponse = sender.ReceiveData();
         string strResponse = Encoding.ASCII.GetString(btarrResponse);

         Assert.IsTrue(strResponse == "ok");
      }
   }
}