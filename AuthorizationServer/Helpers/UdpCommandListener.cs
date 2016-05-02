using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands;
using CoreLib.Entity;
using CoreLib.Serialization;


namespace AuthorizationServer.Helpers {
   /// <summary>
   /// class provides easy methods for listen local udp port and recives tcp settings for client
   /// </summary>
   public class UdpCommandListener {
      private IPEndPoint _RemoteEndPoint;
      private string _TcpEpSettings;
      private int _ListenPort;

      /// <summary>
      /// create instance of UdpHelper
      /// </summary>
      /// <param name="listenPort">local listen port</param>
      /// <param name="tcpEpSettings">local service TCP endpoint</param>
      public UdpCommandListener(int listenPort, string tcpEpSettings) {
         _ListenPort = listenPort;
         _TcpEpSettings = tcpEpSettings;
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
            Parse(data);
            client.Close();
         }
      }

      private void Parse(byte[] data) {
         string result = Encoding.ASCII.GetString(data);
         if(result == "GET SETTINGS") {
            SendTcpSettings();
         }
         else {
            var deserializer = new XmlSerialization<ServiceCommand>();
            var command = deserializer.Deserialize(new MemoryStream(data));
            CommandExecute(command);
         }
      }

      private void CommandExecute(ServiceCommand command) {
         switch(command.Command) {
         case CommandActions.GetAuthorizationInfo:
            GetAuthorizationInfo(command);
            break;
         }
      }

      private void GetAuthorizationInfo(ServiceCommand command) {
         int userId = SessionKeysManager.GetIdByKey(command.SessionKey);
         if(userId != 0) {
            using(var provider = new EntityProvider()) {
               User user = provider.GetUserById(userId);
               var serialization = new XmlSerialization<User>();
               var stream = serialization.Serialize(user);
               SendResponse(stream);
            }
         }
      }

      private void SendResponse(Stream stream) {
         var client = new UdpClient();
         byte[] btarr = new byte[stream.Length];
         stream.Read(btarr, 0, (int)stream.Length);
         client.Connect(_RemoteEndPoint);
         client.Send(btarr, btarr.Length);
      }


      private void SendTcpSettings() {
         var client = new UdpClient();
         var strAddress = _TcpEpSettings.ToString();
         byte[] btarr = Encoding.ASCII.GetBytes(strAddress);
         client.Connect(_RemoteEndPoint);
         client.Send(btarr, btarr.Length);
      }
   }
}