using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Helpers;
using System.ServiceModel;
using System.Xml.Serialization;
using AuthorizationServer.Listeners;
using CoreLib.Entity;

namespace AuthorizationServer {
   class Program {
      static void Main(string[] args) {
         //var btarr = new byte[] { 0, 0, 0, 0, 0, 0, 0, 250 };
         //Int64 v = BitConverter.ToInt64(btarr, 0);
         //var bt = BitConverter.GetBytes(v);

         var localEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
         var setter = new AuthorizationListener(4444, localEp);
         setter.ListenUdp();
         Console.ReadLine();
      }
   }
}