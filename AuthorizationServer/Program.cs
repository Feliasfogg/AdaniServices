using System;
using System.Net;
using AuthorizationServer.Listeners;
using CoreLib.Settings;

namespace AuthorizationServer {
   class Program {
      static void Main(string[] args) {
         ServerSettings serverSettings = ServerSettingsReader.ReadXml();

         //tcp/ip127.0.0.1:1111 udp:4444
         var listener = new AuthorizationListener(serverSettings.UdpPort, serverSettings.TcpEp);
         listener.ListenUdpAsync();
         listener.ListenTcpAsync();
         Console.ReadLine();
      }
   }
}