using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Helpers;
using System.ServiceModel;
using AuthorizationWcfLib;
using AuthorizationWcfLib.Data;

namespace AuthorizationServer {
   class Program {
      static void Main(string[] args) {
         //var btarr = new byte[] { 0, 0, 0, 0, 0, 0, 0, 250 };
         //Int64 v = BitConverter.ToInt64(btarr, 0);
         //var bt = BitConverter.GetBytes(v);

         string serviceAddress = "net.tcp://127.0.0.1:11111";
         var setter = new UdpCommandListener(4444, serviceAddress);
         setter.Listen();

         //ServiceHost host = new ServiceHost(typeof(AuthorizationService), new Uri(serviceAddress));
         //host.AddServiceEndpoint(typeof(IAuthorization), new NetTcpBinding(), string.Empty);
         //host.Open();
         //Console.WriteLine("ServiceStarted at " + serviceAddress);
         //Console.ReadKey();
      }
   }
}