using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Senders {
   public interface ISender {
      void SendBroadcastCommand(string command);
      void SendTcpCommand(string command);
      byte[] ReceiveData();
   }
}
