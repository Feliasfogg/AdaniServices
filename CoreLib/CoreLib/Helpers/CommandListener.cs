using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CoreLib.Commands;
using CoreLib.Entity;
using CoreLib.Serialization;


namespace CoreLib.Helpers {
   /// <summary>
   /// class provides easy methods for listen local udp port and recives tcp settings for client
   /// </summary>
   public abstract class CommandListener : IListener {
      protected IPEndPoint _RemoteEndPoint;
      protected IPEndPoint _LocalTcpEp;
      protected int _ListenPort;

      /// <summary>
      /// create instance of UdpHelper
      /// </summary>
      /// <param name="listenPort">local listen port</param>
      /// <param name="localTcpEp">local service TCP endpoint</param>
      public CommandListener(int listenPort, IPEndPoint localTcpEp) {
         _ListenPort = listenPort;
         _LocalTcpEp = localTcpEp;
         _RemoteEndPoint = new IPEndPoint(IPAddress.Any, 1111);
      }

      public Task ListenAsync() {
         return Task.Run(() => Listen());
      }

      public void Listen() {
         UdpClient client;
         while(true) {
            client = new UdpClient(_ListenPort);
            byte[] data = client.Receive(ref _RemoteEndPoint);
            client.Close();
            Parse(data);
         }
      }

      protected abstract void Parse(byte[] data);
      protected abstract void CommandExecute(ServiceCommand command);

      protected void SendTcpSettings() {
         var strAddress = _LocalTcpEp.ToString();
         byte[] btarr = Encoding.ASCII.GetBytes(strAddress);

         var client = new UdpClient();
         client.Connect(_RemoteEndPoint);
         client.Send(btarr, btarr.Length);
      }
   }
}