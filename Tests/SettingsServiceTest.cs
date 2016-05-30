using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands;
using CoreLib.Commands.Common;
using CoreLib.Commands.Settings;
using CoreLib.Commands.User;
using CoreLib.Entity;
using CoreLib.Helpers;
using CoreLib.Senders;
using CoreLib.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests {
   [TestClass]
   public class SettingsServiceTest {
        LogSender senderlog = new LogSender(BroadcastHelper.GetBroadcastIp(), 4999);
       private string strSessionKey;
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
         strSessionKey = Encoding.ASCII.GetString(bytes);
         return strSessionKey;
      }

      [TestMethod]
      public void GetDeviceInfoTest() {
         strSessionKey = AuthorizeUser();

         var settingsCommandSender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4555);
         settingsCommandSender.GetTcpSettings();

         var deviceSettingsCommand = new SettingsCommand() {
            Command = CommandActions.GetDevice,
            SessionKey = strSessionKey,
            DeviceId = 2
         };

         string xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);

         settingsCommandSender.SendTcpCommand(xmlCommand);
         byte[] bytes = settingsCommandSender.ReceiveData();
         Device device = XmlSerializer<Device>.Deserialize(bytes);
      }

      [TestMethod]
      public void AddDeviceTest() {
          try
          {
              strSessionKey = AuthorizeUser();

              var settingsCommandSender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4555);
              settingsCommandSender.GetTcpSettings();

              var newDevice = new Device()
              {
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

              var deviceSettingsCommand = new SettingsCommand()
              {
                  Command = CommandActions.AddDevice,
                  Device = newDevice,
                  SessionKey = strSessionKey,
              };

              string xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);

              settingsCommandSender.SendTcpCommand(xmlCommand);
              byte[] bytes = settingsCommandSender.ReceiveData();
              Assert.IsTrue(Encoding.ASCII.GetString(bytes) == "ok");
              senderlog.SendString("Add Device complete", strSessionKey);
            }
          catch (Exception ex)
          {
                strSessionKey = "Exception";
                senderlog.SendException(ex, "no Session Key");
            }
      }

      [TestMethod]
      public void EditDeviceTest() {
          try
          {
              strSessionKey = AuthorizeUser();

              var settingsCommandSender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4555);
              settingsCommandSender.GetTcpSettings();

              var deviceSettingsCommand = new SettingsCommand()
              {
                  Command = CommandActions.GetDevice,
                  SessionKey = strSessionKey,
                  DeviceId = 2
              };

              string xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);

              settingsCommandSender.SendTcpCommand(xmlCommand);
              byte[] bytes = settingsCommandSender.ReceiveData();
              Device device = XmlSerializer<Device>.Deserialize(bytes);

              device.ConnectionType = "USB";

              deviceSettingsCommand = new SettingsCommand()
              {
                  Command = CommandActions.EditDevice,
                  SessionKey = strSessionKey,
                  Device = device
              };

              xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);
              settingsCommandSender.SendTcpCommand(xmlCommand);
              bytes = settingsCommandSender.ReceiveData();
              string result = Encoding.ASCII.GetString(bytes);
              Assert.IsTrue(result == "ok");
              senderlog.SendString("Edit Device complete", strSessionKey);
            }
          catch (Exception ex)
          {
                strSessionKey = "Exception";
                senderlog.SendException(ex, "no Session Key");
            }
      }

      [TestMethod]
      public void CreateRemoveDevice() {
          try
          {
              strSessionKey = AuthorizeUser();

              var settingsCommandSender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4555);
              settingsCommandSender.GetTcpSettings();
              //создаем настройки оборудования
              var newDevice = new Device()
              {
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

              var deviceSettingsCommand = new SettingsCommand()
              {
                  Command = CommandActions.AddDevice,
                  Device = newDevice,
                  SessionKey = strSessionKey,
              };

              string xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);

              settingsCommandSender.SendTcpCommand(xmlCommand);
              byte[] bytes = settingsCommandSender.ReceiveData();
              Assert.IsTrue(Encoding.ASCII.GetString(bytes) == "ok");

              deviceSettingsCommand = new SettingsCommand()
              {
                  Command = CommandActions.RemoveDevice,
                  DeviceId = 2,
                  SessionKey = strSessionKey
              };

              xmlCommand = XmlSerializer<SettingsCommand>.SerializeToXmlString(deviceSettingsCommand);

              settingsCommandSender.SendTcpCommand(xmlCommand);
              bytes = settingsCommandSender.ReceiveData();
              string result = Encoding.ASCII.GetString(bytes);
              Assert.IsTrue(result == "ok");
              senderlog.SendString("Create/Remove complete", strSessionKey);
            }
          catch (Exception ex)
          {
                strSessionKey = "Exception";
                senderlog.SendException(ex, "no Session Key");
            }
      }
   }
}