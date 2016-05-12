using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Entity;
using CoreLib.Helpers;
using CoreLib.Serialization;
using DeviceSettingsServer.Listeners;

namespace DeviceSettingsServer {
   class Program {
      static void Main(string[] args) {
         // TODO  {"There was an error reflecting property 'DeviceGroup'."}
         try {
            using(var provider = new EntityProvider()) {
               Device device = provider.GetDeviceInfo(2);
               var d = new Device() {
                  Id = device.Id,
                  Name = device.Name
               };
               var xml = XmlSerializer<Device>.SerializeToXmlString(d);
            }
         }
         catch(Exception ex) {
         }

         var localEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111);

         var listener = new SettingsListener(4555, localEp);
         listener.ListenUdpAsync();
         listener.ListenTcpAsync();
         Console.ReadLine();
      }
   }
}