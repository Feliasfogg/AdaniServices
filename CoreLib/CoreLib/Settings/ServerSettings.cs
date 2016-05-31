using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Settings {
   public class ServerSettings {
      public IPEndPoint TcpEp { get; set; }
      public int UdpPort { get; set; }
      public string DataFolderPath { get; set; }
   }
}