using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Helpers {
   public static class BroadcastHelper {
      public static IPAddress GetBroadcastIp() {
         string ipadress;
         IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName()); // get a list of all local IPs
         IPAddress localIpAddress = ipHostInfo.AddressList[0]; // choose the first of the list
         ipadress = Convert.ToString(localIpAddress); // convert to string
         ipadress = ipadress.Substring(0, ipadress.LastIndexOf(".") + 1); // cuts of the last octet of the given IP 
         ipadress += "255"; // adds 255 witch represents the local broadcast
         IPAddress adress = IPAddress.Parse(ipadress);
         return adress;
      }
   }
}