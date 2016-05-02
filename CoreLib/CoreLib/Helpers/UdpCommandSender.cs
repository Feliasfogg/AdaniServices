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
   public class UdpCommandSender {
      private UdpClient _Client;
      private const string _SettingsRequest = "GET SETTINGS";
      private IPEndPoint _BroadCastAddress;
      private IPEndPoint _RemoteEndPoint;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="broadcastAddress"></param>
      /// <param name="targetPort"></param>
      public UdpCommandSender(string broadcastAddress, int targetPort) {
         _Client = new UdpClient();
         _Client.EnableBroadcast = true;
         _BroadCastAddress = new IPEndPoint(IPAddress.Parse(broadcastAddress), targetPort);
         _RemoteEndPoint = new IPEndPoint(IPAddress.Parse(broadcastAddress), targetPort);
      }

      public UserEntity GetUserInfo(ServiceCommand command) {
         var serializer = new XmlSerialization<ServiceCommand>();
         Stream requestSteam = serializer.Serialize(command);

         byte[] btarrCommand = new byte[requestSteam.Length];
         requestSteam.Read(btarrCommand, 0, (int)requestSteam.Length);
         _Client.Send(btarrCommand, btarrCommand.Length, _BroadCastAddress);

         byte[] btarrResponse = _Client.Receive(ref _RemoteEndPoint);
         var deserializer = new XmlSerialization<UserEntity>();
         var responseStream = new MemoryStream(btarrResponse);
         UserEntity userEntity = deserializer.Deserialize(responseStream);
         return userEntity;
      }

      public string GetTcpSettings() {
         byte[] btarrRequest = Encoding.ASCII.GetBytes(_SettingsRequest);
         _Client.Send(btarrRequest, btarrRequest.Length, _BroadCastAddress);
         byte[] btarrResponse = _Client.Receive(ref _RemoteEndPoint);
         string strResponse = Encoding.ASCII.GetString(btarrResponse);
         return strResponse;
      }
   }
}