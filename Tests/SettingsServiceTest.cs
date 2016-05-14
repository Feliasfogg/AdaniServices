using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands;
using CoreLib.Commands.Authorization;
using CoreLib.Commands.Common;
using CoreLib.Commands.Settings;
using CoreLib.Entity;
using CoreLib.Helpers;
using CoreLib.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests {
   [TestClass]
   public class SettingsServiceTest {
      [TestMethod]
      public void GetTcpSettingsTest() {
         var sender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4555);
         sender.GetTcpSettings();
      }

      public string AuthorizeUser() {
         var accessBytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 255 };
         Int64 accessLevel = BitConverter.ToInt64(accessBytes, 0);
         var sender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4444);
         sender.GetTcpSettings();
         var newUser = new User() {
            Login = "felias",
            Password = "fenris",
            Name = "pavel",
            AccessLevel = accessLevel,
         };
         var addUserCommand = new UserCommand() {
            Command = CommandActions.AddUser,
            User = newUser
         };
         string addUserCommandXml = XmlSerializer<UserCommand>.SerializeToXmlString(addUserCommand);
         sender.SendTcpCommand(addUserCommandXml);
         byte[] bytes = sender.ReceiveData();
         var authCommand = new UserCommand() {
            Command = CommandActions.Authorization,
            Login = "felias",
            Password = "fenris"
         };
         string authCommandXml = XmlSerializer<UserCommand>.SerializeToXmlString(authCommand);
         sender.SendTcpCommand(authCommandXml);
         bytes = sender.ReceiveData();
         string strSessionKey = Encoding.ASCII.GetString(bytes);
         return strSessionKey;
      }

      [TestMethod]
      public void GetDeviceInfoTest() {
         string strSessionKey = AuthorizeUser();

         var settingsCommandSender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4555);
         settingsCommandSender.GetTcpSettings();

         var deviceSettingsCommand = new SettingsCommand() {
            Command = CommandActions.GetDevice,
            SessionKey = strSessionKey,
            DeviceId = 2
         };

         string xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);

         settingsCommandSender.SendTcpCommand(xmlCommand);
         byte[] btarrResponse = settingsCommandSender.ReceiveData();
         Device device = XmlSerializer<Device>.Deserialize(btarrResponse);
      }

      [TestMethod]
      public void AddDeviceTest() {
         string strSessionKey = AuthorizeUser();

         var settingsCommandSender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4555);
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
            SessionKey = strSessionKey,
         };

         string xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);

         settingsCommandSender.SendTcpCommand(xmlCommand);
         byte[] btarrResponse = settingsCommandSender.ReceiveData();
         Assert.IsTrue(Encoding.ASCII.GetString(btarrResponse) == "ok");
      }

      [TestMethod]
      public void EditDeviceTest() {
         string strSessionKey = AuthorizeUser();

         var settingsCommandSender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4555);
         settingsCommandSender.GetTcpSettings();

         var deviceSettingsCommand = new SettingsCommand() {
            Command = CommandActions.GetDevice,
            SessionKey = strSessionKey,
            DeviceId = 2
         };

         string xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);

         settingsCommandSender.SendTcpCommand(xmlCommand);
         byte[] btarrResponse = settingsCommandSender.ReceiveData();
         Device device = XmlSerializer<Device>.Deserialize(btarrResponse);

         device.ConnectionType = "USB";

         deviceSettingsCommand = new SettingsCommand() {
            Command = CommandActions.EditDevice,
            SessionKey = strSessionKey,
            Device = device
         };

         xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);
         settingsCommandSender.SendTcpCommand(xmlCommand);
         btarrResponse = settingsCommandSender.ReceiveData();
         string result = Encoding.ASCII.GetString(btarrResponse);
         Assert.IsTrue(result == "ok");
      }
   }
}