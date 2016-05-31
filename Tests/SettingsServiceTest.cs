using System;
using System.Text;
using CoreLib.Commands.Common;
using CoreLib.Commands.Settings;
using CoreLib.Commands.User;
using CoreLib.Encryption;
using CoreLib.Entity;
using CoreLib.Helpers;
using CoreLib.Senders;
using CoreLib.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests {
   [TestClass]
   public class SettingsServiceTest {
      private LogSender logSender = new LogSender(BroadcastHelper.BroadCastIp, 4999);
      private string sessionKey;

      [TestMethod]
      public void GetTcpSettingsTest() {
         var sender = new CommandSender(BroadcastHelper.BroadCastIp, 4555);
         logSender.SendString("Try get TCP settings for 4555");
         sender.GetTcpSettings();
         logSender.SendString("TCP settings received successfully", sessionKey);
      }

      public string AuthorizeUser() {
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
         return sessionKey;
      }

      [TestMethod]
      public void GetDeviceInfoTest() {
         try {
            sessionKey = AuthorizeUser();

            var settingsCommandSender = new CommandSender(BroadcastHelper.BroadCastIp, 4555);
            settingsCommandSender.GetTcpSettings();

            var deviceSettingsCommand = new SettingsCommand() {
               Command = CommandActions.GetDevice,
               SessionKey = sessionKey,
               DeviceId = 4
            };

            string xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);

            settingsCommandSender.SendTcpCommand(xmlCommand);
            logSender.SendString($"Try get device info for deviceId:{deviceSettingsCommand.DeviceId}", sessionKey);
            byte[] bytes = settingsCommandSender.ReceiveData();
            Device device = XmlSerializer<Device>.Deserialize(bytes);
            logSender.SendString($"Device settings received succesfully", sessionKey);
         }
         catch(Exception ex) {
            logSender.SendException(ex, sessionKey);
         }
      }

      [TestMethod]
      public void AddDeviceTest() {
         try {
            sessionKey = AuthorizeUser();

            var settingsCommandSender = new CommandSender(BroadcastHelper.BroadCastIp, 4555);
            settingsCommandSender.GetTcpSettings();

            var newDevice = new Device() {
               DeviceGroupId = 1,
               ConnectionType = "COM",
               GeneratorType = 2,
               NormalVoltage = 200,
               HighVoltage = 200,
               NormalCurrent = 175,
               HighCurrent = 200,
               HighMode = 0,
               ReseasonDate = 422.111,
               WorkTime = 1.365,
               XRayTime = 0.512,
               LastWorkedDate = 422.111,
               Name = "H-projection"
            };

            var deviceSettingsCommand = new SettingsCommand() {
               Command = CommandActions.AddDevice,
               Device = newDevice,
               SessionKey = sessionKey,
            };

            string xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);
            logSender.SendString($"Try create new device", sessionKey);
            settingsCommandSender.SendTcpCommand(xmlCommand);
            byte[] bytes = settingsCommandSender.ReceiveData();
            Assert.IsTrue(Encoding.ASCII.GetString(bytes) == "ok");
            logSender.SendString("New device created succesfully", sessionKey);
         }
         catch(Exception ex) {
            logSender.SendException(ex, sessionKey);
         }
      }

      [TestMethod]
      public void EditDeviceTest() {
         try {
            sessionKey = AuthorizeUser();

            var settingsCommandSender = new CommandSender(BroadcastHelper.BroadCastIp, 4555);
            settingsCommandSender.GetTcpSettings();

            var deviceSettingsCommand = new SettingsCommand() {
               Command = CommandActions.GetDevice,
               SessionKey = sessionKey,
               DeviceId = 4
            };

            string xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);
            logSender.SendString($"Try get device info for deviceId:{deviceSettingsCommand.DeviceId}", sessionKey);
            settingsCommandSender.SendTcpCommand(xmlCommand);
            byte[] bytes = settingsCommandSender.ReceiveData();
            Device device = XmlSerializer<Device>.Deserialize(bytes);
            logSender.SendString($"Device info received succesfully", sessionKey);
            device.ConnectionType = "USB";

            deviceSettingsCommand = new SettingsCommand() {
               Command = CommandActions.EditDevice,
               SessionKey = sessionKey,
               Device = device
            };

            xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);
            logSender.SendString($"Try edit device info for {deviceSettingsCommand.Device.Id}", sessionKey);
            settingsCommandSender.SendTcpCommand(xmlCommand);
            bytes = settingsCommandSender.ReceiveData();
            string result = Encoding.ASCII.GetString(bytes);
            Assert.IsTrue(result == "ok");
            logSender.SendString("Edit Device completed succesfully", sessionKey);
         }
         catch(Exception ex) {
            logSender.SendException(ex, sessionKey);
         }
      }

      [TestMethod]
      public void CreateRemoveDevice() {
         try {
            sessionKey = AuthorizeUser();

            var settingsCommandSender = new CommandSender(BroadcastHelper.BroadCastIp, 4555);
            settingsCommandSender.GetTcpSettings();
            //создаем настройки оборудования
            var newDevice = new Device() {
               DeviceGroupId = 1,
               ConnectionType = "COM",
               GeneratorType = 2,
               NormalVoltage = 200,
               HighVoltage = 200,
               NormalCurrent = 175,
               HighCurrent = 200,
               HighMode = 0,
               ReseasonDate = 422.111,
               WorkTime = 1.365,
               XRayTime = 0.512,
               LastWorkedDate = 422.111,
               Name = "H-projection"
            };

            var deviceSettingsCommand = new SettingsCommand() {
               Command = CommandActions.AddDevice,
               Device = newDevice,
               SessionKey = sessionKey,
            };

            string xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);
            logSender.SendString("Try add new device", sessionKey);
            settingsCommandSender.SendTcpCommand(xmlCommand);
            byte[] bytes = settingsCommandSender.ReceiveData();
            Assert.IsTrue(Encoding.ASCII.GetString(bytes) == "ok");
            logSender.SendString($"Device created successfully", sessionKey);
            deviceSettingsCommand = new SettingsCommand() {
               Command = CommandActions.RemoveDevice,
               DeviceId = 7,
               SessionKey = sessionKey
            };

            xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);
            logSender.SendString($"Try remove device deviceId{deviceSettingsCommand.DeviceId}", sessionKey);
            settingsCommandSender.SendTcpCommand(xmlCommand);
            bytes = settingsCommandSender.ReceiveData();
            string result = Encoding.ASCII.GetString(bytes);
            Assert.IsTrue(result == "ok");
            logSender.SendString("Create/Remove device completed succesfully", sessionKey);
         }
         catch(Exception ex) {
            logSender.SendException(ex, sessionKey);
         }
      }

      [TestMethod]
      public void ExportDatabase() {
         try {
            sessionKey = AuthorizeUser();

            var settingsCommandSender = new CommandSender(BroadcastHelper.BroadCastIp, 4555);
            settingsCommandSender.GetTcpSettings();

            var deviceSettingsCommand = new SettingsCommand() {
               Command = CommandActions.ExportDataBase,
               SessionKey = sessionKey,
            };

            string xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);

            settingsCommandSender.SendTcpCommand(xmlCommand);
            logSender.SendString($"Try export database:{deviceSettingsCommand.DeviceId}", sessionKey);
            byte[] bytes = settingsCommandSender.ReceiveData();
            string data = Encoding.ASCII.GetString(bytes);
            logSender.SendString($"Database exported succesfully", sessionKey);
         }
         catch(Exception ex) {
            logSender.SendException(ex, sessionKey);
         }
      }
   }
}