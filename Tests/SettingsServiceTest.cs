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
      public void GetDeviceSettingsTest() {
         var authSender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4444);
         authSender.GetTcpSettings();

         var command = new AuthorizationCommand() {
            Command = CommandActions.Authorization,
            Login = "felias",
            Password = "fenris"
         };

         var serializer1 = new XmlSerializer<AuthorizationCommand>();
         string commandString = serializer1.SerializeToXmlString(command);

         authSender.SendCommand(commandString);
         byte[] btarrResponse = authSender.ReceiveData();
         string strResponse = Encoding.ASCII.GetString(btarrResponse);

         var deviceSettingsCommand = new DeviceSettingsCommand() {
            Command = CommandActions.GetDeviceSettings,
            SessionKey = strResponse
         };

         var serializer2 = new XmlSerializer<DeviceSettingsCommand>();
         string strDeviceSettingsCommand = serializer2.SerializeToXmlString(deviceSettingsCommand);

         var settingsCommandSender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4555);
         settingsCommandSender.GetTcpSettings();

         settingsCommandSender.SendCommand(strDeviceSettingsCommand);
         btarrResponse = settingsCommandSender.ReceiveData();
         string strAuthInfoResult = Encoding.ASCII.GetString(btarrResponse);
         Assert.IsTrue(strAuthInfoResult != String.Empty);
      }
   }
}
