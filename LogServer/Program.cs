using System;
using System.Net;
using CoreLib.Settings;
using LogServer.Listeners;

namespace LogServer {
   class Program {
      static void Main(string[] args) {
         //tcp/ip 127.0.0.1:13000 udp-port: 4999
         ServerSettings serverSettings = ServerSettingsReader.ReadXml();

         LogListener listenerobj = new LogListener(serverSettings.UdpPort, serverSettings.TcpEp, serverSettings.DataFolderPath);
         listenerobj.ListenUdpAsync();
         Console.ReadLine();
      }
   }
}