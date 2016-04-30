using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Helpers;
using System.ServiceModel;
using AuthorizationWcfLib;

namespace AuthorizationServer {
   class Program {
      static void Main(string[] args) {
         var setter = new UdpListener(4444, "net.tcp://127.0.0.1:11000");
         setter.Listen();

         //string serviceAddress = "net.tcp://127.0.0.1:6666";

         //ServiceHost host = new ServiceHost(typeof(AuthorizationService), new Uri(serviceAddress));
         //host.AddServiceEndpoint(typeof(IAuthorization), new NetTcpBinding(), string.Empty);
         //host.Open();
         //Console.ReadKey();
      }
   }
}