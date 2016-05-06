using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CoreLib.Commands;
using CoreLib.Commands.Common;
using CoreLib.Commands.Settings;
using CoreLib.Entity;
using CoreLib.Helpers;
using CoreLib.Serialization;

namespace DeviceSettingsServer.Listeners {
   public class SettingsListener : CommandListener {
      public SettingsListener(int listenPort, IPEndPoint localTcpEp) : base(listenPort, localTcpEp) {
      }

      protected override async Task Parse(byte[] data) {
         string strData = Encoding.ASCII.GetString(data);
         if(strData == "GET SETTINGS") {
            base.SendTcpSettings();
         }
         else {
            var xml = new XmlDocument();
            xml.LoadXml(strData);
            XmlNodeList nodeList = xml.GetElementsByTagName("Command");
            var xmlNode = nodeList.Item(0);
            var responser = new Responser(_LocalTcpEp);
            switch(xmlNode.InnerText) {
            case "GetDeviceSettings": {
               await CheckAuthorization(data);
            }
               break;
            default:
               break;
            }
         }
      }

      private async Task CheckAuthorization(byte[] data) {
         var deserializer = new XmlSerializer<DeviceSettingsCommand>();
         var command = deserializer.Deserialize(new MemoryStream(data));

         var sender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4444);
         sender.GetTcpSettings();

         var authInfoCommand = new ServiceCommand() {
            Command = CommandActions.AuthorizationInfo,
            SessionKey = command.SessionKey
         };

         var serializer = new XmlSerializer<ServiceCommand>();
         string strAuthInfoCommand = serializer.SerializeToXmlString(authInfoCommand);

         sender.SendCommand(strAuthInfoCommand);
         byte[] btarrResponse = await sender.ReceiveDataAsync();
         string strAuthInfoResult = Encoding.ASCII.GetString(btarrResponse);


         if(strAuthInfoResult != String.Empty) {
           SendResponse(Encoding.ASCII.GetBytes("ok"));
         }
         else {
           SendResponse(Encoding.ASCII.GetBytes("error"));
         }
      }
   }
}