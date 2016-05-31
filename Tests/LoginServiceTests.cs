using System;
using System.IO;
using System.Text;
using CoreLib.Helpers;
using CoreLib.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoreLib.Commands.Common;
using CoreLib.Commands.User;
using CoreLib.Encryption;
using CoreLib.Entity;
using CoreLib.Senders;

namespace Tests {
   [TestClass]
   public class LoginServiceTests {
      private LogSender logSender = new LogSender(BroadcastHelper.BroadCastIp, 4999);
      private string sessionKey = String.Empty;

      public string AuthorizeUser() {
         LogSender senderlog = new LogSender(BroadcastHelper.BroadCastIp, 4999);
         string sessionKey;
         try {
            //здесь кусок кода из теста AuthorizationTest, вынес сюда для сокращения объемов других тестов
            var accessBytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 255 };
            Int64 accessLevel = BitConverter.ToInt64(accessBytes, 0);
            var sender = new CommandSender(BroadcastHelper.BroadCastIp, 4444);
            sender.GetTcpSettings();
            var newUser = new User() {
               Login = "felias",
               PasswordHash = Encrypter.GenerateHash("fenris"),
               Name = "pavel",
               AccessLevel = accessLevel,
            };

            var addUserCommand = new UserCommand() {
               Command = CommandActions.AddUser,
               User = newUser
            };
            senderlog.SendString("Try add new user");
            string addUserCommandXml = XmlSerializer<UserCommand>.SerializeToXmlString(addUserCommand);
            sender.SendTcpCommand(addUserCommandXml);
            byte[] bytes = sender.ReceiveData();
            if(Encoding.ASCII.GetString(bytes) == "ok") {
               senderlog.SendString("New user addded succesfully");
            }

            var authCommand = new UserCommand() {
               Command = CommandActions.Authorization,
               Login = "felias",
               PasswordHash = Encrypter.GenerateHash("fenris")
            };
            string authCommandXml = XmlSerializer<UserCommand>.SerializeToXmlString(authCommand);
            senderlog.SendString($"Try authorization for {authCommand.Login}");
            sender.SendTcpCommand(authCommandXml);
            bytes = sender.ReceiveData();
            sessionKey = Encoding.ASCII.GetString(bytes);
            senderlog.SendString("Authorization completed succesfully", sessionKey);
            return sessionKey;
         }
         catch(Exception ex) {
            sessionKey = "No Session Key";
            senderlog.SendException(ex, sessionKey);
            return sessionKey;
         }
      }

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
         try {
            {
               var sender = new CommandSender(BroadcastHelper.BroadCastIp, 4444);
               logSender.SendString("Try get TCP settings for 4444");
               sender.GetTcpSettings();
               logSender.SendString("TCP settings received successfully", sessionKey);
            }
         }
         catch(Exception ex) {
            logSender.SendException(ex, sessionKey);
         }
      }

      [TestMethod]
      public void AuthorizationTest() {
         try {
            var accessBytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 255 };
            Int64 accessLevel = BitConverter.ToInt64(accessBytes, 0);

            var sender = new CommandSender(BroadcastHelper.BroadCastIp, 4444);
            sender.GetTcpSettings();
            //создание юзера
            var newUser = new User() {
               Login = "felias",
               PasswordHash = Encrypter.GenerateHash("fenris"),
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
            logSender.SendString("Try add new user");
            sender.SendTcpCommand(addUserCommandXml);
            byte[] bytes = sender.ReceiveData(); //получение ответа
            Assert.IsTrue(Encoding.ASCII.GetString(bytes) == "ok");
            logSender.SendString("New user added succesfully");
            //команда авторизации
            var authCommand = new UserCommand() {
               Command = CommandActions.Authorization,
               Login = "felias",
               PasswordHash = Encrypter.GenerateHash("fenris")
            };

            string authCommandXml = XmlSerializer<UserCommand>.SerializeToXmlString(authCommand);
            logSender.SendString($"Try authorization for {authCommand.Login}");
            sender.SendTcpCommand(authCommandXml);
            //отрпавка команды на авторизацию, в ответ от сервера должен прийти сессионный ключ авторизации
            bytes = sender.ReceiveData();
            sessionKey = Encoding.ASCII.GetString(bytes);
            logSender.SendString("Authorization completed succesfully", sessionKey);
         }
         catch(Exception ex) {
            logSender.SendException(ex, sessionKey);
         }
      }

      [TestMethod]
      public void GetAuthInfoTest() {
         try {
            var accessBytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 255 };
            Int64 accessLevel = BitConverter.ToInt64(accessBytes, 0);
            var sender = new CommandSender(BroadcastHelper.BroadCastIp, 4444);
            sender.GetTcpSettings();
            var newUser = new User() {
               Login = "felias",
               PasswordHash = Encrypter.GenerateHash("fenris"),
               Name = "pavel",
               AccessLevel = accessLevel,
            };
            var addUserCommand = new UserCommand() {
               Command = CommandActions.AddUser,
               User = newUser
            };
            string addUserCommandXml = XmlSerializer<UserCommand>.SerializeToXmlString(addUserCommand);
            logSender.SendString("Try add new user");
            sender.SendTcpCommand(addUserCommandXml);
            byte[] bytes = sender.ReceiveData();
            Assert.IsTrue(Encoding.ASCII.GetString(bytes) == "ok");
            logSender.SendString("New user added succesfully");

            var authCommand = new UserCommand() {
               Command = CommandActions.Authorization,
               Login = "felias",
               PasswordHash = Encrypter.GenerateHash("fenris")
            };
            string authCommandXml = XmlSerializer<UserCommand>.SerializeToXmlString(authCommand);
            logSender.SendString($"Try authorization for {authCommand.Login}");
            sender.SendTcpCommand(authCommandXml);
            bytes = sender.ReceiveData();
            sessionKey = Encoding.ASCII.GetString(bytes);
            logSender.SendString("Authorization completed succesfully", sessionKey);

            //команда получения инфы о пользователе
            var userInfoCommand = new ServiceCommand() {
               Command = CommandActions.GetUser,
               SessionKey = sessionKey,
            };

            string userInfoCommandXml = XmlSerializer<ServiceCommand>.SerializeToXmlString(userInfoCommand);
            logSender.SendString($"Try get user info", sessionKey);
            sender.SendTcpCommand(userInfoCommandXml);
            bytes = sender.ReceiveData();
            string userInfoXml = Encoding.ASCII.GetString(bytes); //инфа о пользователе
            logSender.SendString($"User info received succefully", sessionKey);
            User user = XmlSerializer<User>.Deserialize(userInfoXml); //десериализация строки инфы о пользователе в объект
            Assert.IsTrue(user.Login == authCommand.Login && user.PasswordHash == authCommand.PasswordHash);
            logSender.SendString("Authorization completed succesfully", sessionKey);
         }
         catch(Exception ex) {
            logSender.SendException(ex, sessionKey);
         }
      }

      [TestMethod]
      public void EditUserTest() {
         try {
            sessionKey = AuthorizeUser();
            var sender = new CommandSender(BroadcastHelper.BroadCastIp, 4444);
            sender.GetTcpSettings();

            //команда получения инфы об авторизации
            var userInfoCommand = new ServiceCommand() {
               Command = CommandActions.GetUser,
               SessionKey = sessionKey
            };

            string userInfoCommandXml = XmlSerializer<ServiceCommand>.SerializeToXmlString(userInfoCommand);
            logSender.SendString($"Try get user info", sessionKey);
            sender.SendTcpCommand(userInfoCommandXml);
            byte[] btarrResponse = sender.ReceiveData();
            string strUserInfo = Encoding.ASCII.GetString(btarrResponse); //get user information
            User user = XmlSerializer<User>.Deserialize(strUserInfo);
            logSender.SendString($"User info received succesfully", sessionKey);

            //изменяем инфу о пользоателе
            user.Login = "newlogin";
            user.PasswordHash = Encrypter.GenerateHash("newpasword");

            //command for edit user info
            var editCommand = new UserCommand() {
               Command = CommandActions.EditUser,
               User = user
            };
            string editCommandXml = XmlSerializer<UserCommand>.SerializeToXmlString(editCommand);
            logSender.SendString($"Try edit user info", sessionKey);
            sender.SendTcpCommand(editCommandXml);
            btarrResponse = sender.ReceiveData();
            string strResponse = Encoding.ASCII.GetString(btarrResponse);
            Assert.IsTrue(strResponse == "ok");
            logSender.SendString($"User info edit succesfully", sessionKey);
         }
         catch(Exception ex) {
            logSender.SendException(ex, sessionKey);
         }
      }

      [TestMethod]
      public void AddDeleteUserTest() {
         try {
            sessionKey = AuthorizeUser();
            var sender = new CommandSender(BroadcastHelper.BroadCastIp, 4444);
            sender.GetTcpSettings();

            //команда получения инфы о пользователе
            var userInfoCommand = new ServiceCommand() {
               Command = CommandActions.GetUser,
               SessionKey = sessionKey,
            };

            string userInfoCommandXml = XmlSerializer<ServiceCommand>.SerializeToXmlString(userInfoCommand);
            logSender.SendString($"Try get user info", sessionKey);
            sender.SendTcpCommand(userInfoCommandXml);
            byte[] bytes = sender.ReceiveData();
            string strUserInfo = Encoding.ASCII.GetString(bytes); //инфа о пользователе
            User user = XmlSerializer<User>.Deserialize(strUserInfo); //десериализация строки инфы о пользователе в объект
            logSender.SendString($"User info received succesfully", sessionKey);

            //удаление пользователя
            var deleteCommand = new UserCommand() {
               Command = CommandActions.RemoveUser,
               UserId = user.Id
            };

            string deleteCommandXml = XmlSerializer<UserCommand>.SerializeToXmlString(deleteCommand);
            logSender.SendString($"Try delete user info", sessionKey);
            sender.SendTcpCommand(deleteCommandXml);
            bytes = sender.ReceiveData();
            Assert.IsTrue(Encoding.ASCII.GetString(bytes) == "ok");
            logSender.SendString("User deleted succesfully", sessionKey);
         }
         catch(Exception ex) {
            logSender.SendException(ex, sessionKey);
         }
      }
   }
}