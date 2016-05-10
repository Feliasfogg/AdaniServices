using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Helpers;
using DeviceSettingsServer.Listeners;

namespace DeviceSettingsServer {
   class Program {
      static void Main(string[] args) {
         var localEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111);

         var listener = new SettingsListener(4555, localEp);
         listener.ListenUdp();
         Console.ReadLine();
      }
   }
}
