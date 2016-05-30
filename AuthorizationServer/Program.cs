using System;
using System.Net;
using AuthorizationServer.Listeners;

namespace AuthorizationServer {
   class Program {
      static void Main(string[] args) {
         //var btarr = new byte[] { 0, 0, 0, 0, 0, 0, 0, 250 };
         //Int64 v = BitConverter.ToInt64(btarr, 0);
         //var bt = BitConverter.GetBytes(v);

         var localEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
         var setter = new AuthorizationListener(4444, localEp);
         setter.ListenUdpAsync();
         setter.ListenTcpAsync();
         Console.ReadLine();
      }
   }
}