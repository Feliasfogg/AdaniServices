using System;
using System.Net;
using CoreLib.Settings;
using DeviceSettingsServer.Listeners;

namespace DeviceSettingsServer {
   class Program {
      static void Main(string[] args) {
         //tcp/ip 127.0.0.1:12000 udp port:4555
         ServerSettings serverSettings = ServerSettingsReader.ReadXml();

         var listener = new SettingsListener(serverSettings.UdpPort, serverSettings.TcpEp);
         listener.ListenUdpAsync();
         listener.ListenTcpAsync();
         Console.ReadLine();
      }
   }
}