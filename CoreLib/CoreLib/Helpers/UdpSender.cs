using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands;
using CoreLib.Serialization;

namespace CoreLib.Helpers {
   public class UdpSender {
      private UdpClient _Client;
      private const string _SettingsRequest = "GET SETTINGS";
      private IPEndPoint _BroadCastAddress;

      /// <summary>
      /// create instance of TcpSettingsHelper
      /// </summary>
      /// <param name="targetPort">remote target port for broadcast</param>
      public UdpSender(string strBroadCastAdress, int targetPort ) {
         _Client = new UdpClient();
         _Client.EnableBroadcast = true;
         _BroadCastAddress = new IPEndPoint(IPAddress.Parse(strBroadCastAdress), targetPort);
      }
      public void SendCommand(ServiceCommand command) {
         var serializer = new XmlSerialization<ServiceCommand>();

         Stream stream = serializer.Serialize(command);
         byte[] btarrCommand = new byte[stream.Length];
         stream.Read(btarrCommand, 0, (int)stream.Length);
         _Client.Send(btarrCommand, btarrCommand.Length, _BroadCastAddress);
      }
      public string RequireTcpSettings() {
         var remoteEndPoint = new IPEndPoint(IPAddress.Any, 1111);
         byte[] btarrRequest = Encoding.ASCII.GetBytes(_SettingsRequest);
         _Client.Send(btarrRequest, btarrRequest.Length, _BroadCastAddress);
         byte[] btarrResponse = _Client.Receive(ref remoteEndPoint);
         string strResponse = Encoding.ASCII.GetString(btarrResponse);
         return strResponse;
      }
   }
}