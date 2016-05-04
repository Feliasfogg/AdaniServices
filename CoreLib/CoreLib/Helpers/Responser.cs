using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands;
using CoreLib.Entity;
using CoreLib.Serialization;

namespace CoreLib.Helpers {
   public class Responser {
      private IPEndPoint _TcpLocalEp;

      /// <summary>
      /// create instance of local TcpServer for seding response array via Tcp
      /// </summary>
      /// <param name="localEp">local end point</param>
      public Responser(IPEndPoint localEp) {
         _TcpLocalEp = localEp;
      }

      public void SendResponse(byte[] btarr) {
         var listener = new TcpListener(_TcpLocalEp);
         listener.Start();
         TcpClient client = listener.AcceptTcpClient();
         using(NetworkStream networkStream = client.GetStream()) {
            networkStream.Write(btarr, 0, btarr.Length);
         }
         listener.Stop();
      }
   }
}