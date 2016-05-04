using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CoreLib.Commands;
using CoreLib.Entity;
using CoreLib.Serialization;


namespace CoreLib.Helpers {
   /// <summary>
   /// class provides easy methods for listen local udp port and recives tcp settings for client
   /// </summary>
   public class CommandListener : IListener {
      private IPEndPoint _RemoteEndPoint;
      private IPEndPoint _TcpEp;
      private int _ListenPort;

      /// <summary>
      /// create instance of UdpHelper
      /// </summary>
      /// <param name="listenPort">local listen port</param>
      /// <param name="tcpEp">local service TCP endpoint</param>
      public CommandListener(int listenPort, IPEndPoint tcpEp) {
         _ListenPort = listenPort;
         _TcpEp = tcpEp;
         _RemoteEndPoint = new IPEndPoint(IPAddress.Any, 1111);
      }

      public Task ListenUdpAsync() {
         return Task.Run(() => ListenUdp());
      }

      public void ListenUdp() {
         UdpClient client;
         while(true) {
            client = new UdpClient(_ListenPort);
            byte[] data = client.Receive(ref _RemoteEndPoint);
            client.Close();
            Parse(data);
         }
      }

      private void Parse(byte[] data) {
         string strData = Encoding.ASCII.GetString(data);
         if(strData == "GET SETTINGS") {
            SendTcpSettings();
         }
         else {
            var xml = new XmlDocument();
            xml.LoadXml(strData);
            XmlNodeList nodeList = xml.GetElementsByTagName("Command");
            var xmlNode = nodeList.Item(0);
            var responser = new Responser(_TcpEp);
            switch(xmlNode.InnerText) {
            case "Authorization": {
               var deserializer = new XmlSerialization<AuthorizationCommand>();
               var command = deserializer.Deserialize(new MemoryStream(data));
               responser.CommandExecute(command);
            }
               break;
            case "AuthorizationInfo": {
               var deserializer = new XmlSerialization<ServiceCommand>();
               var command = deserializer.Deserialize(new MemoryStream(data));
               responser.CommandExecute(command);
            }
               break;
            default:
               break;
            }
         }
      }

      private void SendTcpSettings() {
         var strAddress = _TcpEp.ToString();
         byte[] btarr = Encoding.ASCII.GetBytes(strAddress);

         var client = new UdpClient();
         client.Connect(_RemoteEndPoint);
         client.Send(btarr, btarr.Length);
      }
   }
}