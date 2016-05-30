using System;
using System.Net;
using LogServer.Listeners;

namespace LogServer {
   class Program {
      static void Main(string[] args) {
         var localEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 13000);
         LogListener listenerobj = new LogListener(4999, localEp);
         listenerobj.ListenUdpAsync();
         Console.ReadLine();
      }
   }
}