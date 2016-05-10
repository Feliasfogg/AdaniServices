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

      protected override void Parse(byte[] data) {
         string strData = Encoding.ASCII.GetString(data);
         if(strData == "GET SETTINGS") {
            base.SendTcpSettings();
         }
         else {
            var xml = new XmlDocument();
            xml.LoadXml(strData);
            XmlNodeList nodeList = xml.GetElementsByTagName("Command");
            var xmlNode = nodeList.Item(0);
            switch(xmlNode.InnerText) {
            case "GetDeviceSettings": {
               CheckAuthorization(data);
            }
               break;
            default:
               break;
            }
         }
      }

      private void CheckAuthorization(byte[] data) {
         //блок получения инфы о пользователе на основе переданного сессионного ключа
         var deserializer = new XmlSerializer<DeviceSettingsCommand>();
         var command = deserializer.Deserialize(new MemoryStream(data));

         var sender = new CommandSender(BroadcastHelper.GetBroadcastIp(), 4444);
         sender.GetTcpSettings();

         var authInfoCommand = new ServiceCommand() {
            Command = CommandActions.GetUserInfo,
            SessionKey = command.SessionKey
         };

         var serializer = new XmlSerializer<ServiceCommand>();
         string strAuthInfoCommand = serializer.SerializeToXmlString(authInfoCommand);

         sender.SendUdpCommand(strAuthInfoCommand);
         byte[] btarrResponse = sender.ReceiveData();
         string strAuthInfoResult = Encoding.ASCII.GetString(btarrResponse);
         if (strAuthInfoResult == "error") {
            SendResponse(Encoding.ASCII.GetBytes("error"));
         }

         var serizalizer2 = new XmlSerializer<UserInfo>();
         //десериализация xml в объект пользователя
         var userInfo = serizalizer2.Deserialize(btarrResponse);
         SendResponse("ok");
      }
   }
}