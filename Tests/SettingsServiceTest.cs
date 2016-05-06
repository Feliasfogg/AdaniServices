﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands;
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
      public async Task GetDeviceSettingsTest() {
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
         byte[] btarrResponse =await authSender.ReceiveDataAsync();
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
         btarrResponse = await settingsCommandSender.ReceiveDataAsync();
         string strAuthInfoResult = Encoding.ASCII.GetString(btarrResponse);
         Assert.IsTrue(strAuthInfoResult != String.Empty);
      }
   }
}