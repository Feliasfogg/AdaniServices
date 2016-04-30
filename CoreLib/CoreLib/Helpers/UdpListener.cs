using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands;
using CoreLib.Serialization;


namespace CoreLib.Helpers {
   /// <summary>
   /// class provides easy methods for listen local udp port and recives tcp settings for client
   /// </summary>
   public class UdpListener {
      private IPEndPoint _RemoteEndPoint;
      private string _SettingsTcpEndPoint;
      private int _ListenPort;

      /// <summary>
      /// create instance of UdpHelper
      /// </summary>
      /// <param name="listenPort">local listen port</param>
      /// <param name="settingsTcpEndPoint">local service TCP endpoint</param>
      public UdpListener(int listenPort, string settingsTcpEndPoint) {
         _ListenPort = listenPort;
         _SettingsTcpEndPoint = settingsTcpEndPoint;
         _RemoteEndPoint = new IPEndPoint(IPAddress.Any, 1111);
      }

      public Task ListenAsync() {
         return Task.Run(() => Listen());
      }

      public void Listen() {
         UdpClient client;
         while(true) {
            client = new UdpClient(_ListenPort);
            byte[] data = client.Receive(ref _RemoteEndPoint);
            Parse(data);
            client.Close();
         }
      }

      private void Parse(byte[] data) {
         string result = Encoding.ASCII.GetString(data);
         if(result == "GET SETTINGS") {
            SendSettings();
         }
         else {
            var deserializer = new XmlSerialization<ServiceCommand>();
            var command = deserializer.Deserialize(new MemoryStream(data));
         }
      }

      private void SendSettings() {
         var client = new UdpClient();
         var strAddress = _SettingsTcpEndPoint.ToString();
         byte[] btarr = Encoding.ASCII.GetBytes(strAddress);
         client.Connect(_RemoteEndPoint);
         client.Send(btarr, btarr.Length);
      }
   }
}