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

      [TestMethod]
      public void GetDeviceInfoTest() {
         var authSender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4444);
         authSender.GetTcpSettings();

         var authCommand = new AuthorizationCommand() {
            Command = CommandActions.Authorization,
            Login = "felias",
            Password = "fenris"
         };

         string authCommandXml = XmlSerializer<AuthorizationCommand>.SerializeToXmlString(authCommand);

         authSender.SendUdpCommand(authCommandXml);
         byte[] btarrResponse = authSender.ReceiveData();
         string strSessionKey = Encoding.ASCII.GetString(btarrResponse);
         Assert.IsTrue(strSessionKey != String.Empty && strSessionKey != "error");

         var settingsCommandSender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4555);
         settingsCommandSender.GetTcpSettings();

         var deviceSettingsCommand = new DeviceSettingsCommand() {
            Command = CommandActions.GetDeviceSettings,
            SessionKey = strSessionKey,
            DeviceId = 2
         };

         string deviceSettingsCommandXml = XmlSerializer<DeviceSettingsCommand>.SerializeToXmlString(deviceSettingsCommand);

         settingsCommandSender.SendUdpCommand(deviceSettingsCommandXml);
         btarrResponse = settingsCommandSender.ReceiveData();
         Assert.IsTrue(Encoding.ASCII.GetString(btarrResponse) != "error");
         DeviceEntity device = XmlSerializer<DeviceEntity>.Deserialize(btarrResponse);
      }
   }
}
