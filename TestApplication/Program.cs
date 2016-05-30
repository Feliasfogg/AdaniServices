using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands.Common;
using CoreLib.Commands.Settings;
using CoreLib.Commands.User;
using CoreLib.Entity;
using CoreLib.Helpers;
using CoreLib.Senders;
using CoreLib.Serialization;

namespace TestApplication {
   class Program {
      static void Main(string[] args) {
         LogSender senderlog = new LogSender(BroadcastHelper.BroadCastIp, 4999);
         var accessBytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 255 };
         Int64 accessLevel = BitConverter.ToInt64(accessBytes, 0);
         string sessionKey = String.Empty;

         try {
            var authorizeCommandSender = new CommandSender(BroadcastHelper.BroadCastIp, 4444);
            authorizeCommandSender.GetTcpSettings();
            //создание юзера
            var newUser = new User() {
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
            senderlog.SendString("Try create user", "");
            authorizeCommandSender.SendTcpCommand(addUserCommandXml);
            byte[] bytes = authorizeCommandSender.ReceiveData(); //получение ответа
            if(Encoding.ASCII.GetString(bytes) == "ok") {
               senderlog.SendString("User creates succesfully", "");
            }

            //команда авторизации
            var authCommand = new UserCommand() {
               Command = CommandActions.Authorization,
               Login = "felias",
               Password = "fenris"
            };

            string authCommandXml = XmlSerializer<UserCommand>.SerializeToXmlString(authCommand);
            senderlog.SendString("Try get session key", "");
            authorizeCommandSender.SendTcpCommand(authCommandXml);
            //отрпавка команды на авторизацию, в ответ от сервера должен прийти сессионный ключ авторизации
            bytes = authorizeCommandSender.ReceiveData();
            sessionKey = Encoding.ASCII.GetString(bytes);
            senderlog.SendString("Authorization complete", sessionKey);
            //создание сендера для сервера настроек
            var settingsCommandSender = new CommandSender(BroadcastHelper.BroadCastIp, 4555);
            settingsCommandSender.GetTcpSettings();
            //команда настроек оборудования
            var deviceSettingsCommand = new SettingsCommand() {
               Command = CommandActions.GetDevice,
               SessionKey = sessionKey,
               DeviceId = 3 //должен быть девайс с таким id
            };

            string xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);
            senderlog.SendString("Try get settings", sessionKey);
            settingsCommandSender.SendTcpCommand(xmlCommand);
            bytes = settingsCommandSender.ReceiveData();
            senderlog.SendString("Settings received successfully", sessionKey);
            Device device = XmlSerializer<Device>.Deserialize(bytes);
         }
         catch(Exception ex) {
            senderlog.SendException(ex, sessionKey);
         }
      }
   }
}