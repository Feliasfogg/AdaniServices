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
using Microsoft.SqlServer.Server;

namespace CoreLib.Helpers {
   public class CommandSender : ISender {
      private UdpClient _UdpClient;
      private const string _SettingsRequest = "GET SETTINGS";
      private IPEndPoint _BroadCastAddress;
      private IPEndPoint _RemoteUdpEndPoint;
      private IPEndPoint _RemoteTcpEndPoint;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="broadcastAddress"></param>
      /// <param name="targetPort"></param>
      public CommandSender(string broadcastAddress, int targetPort) {
         _UdpClient = new UdpClient();
         _UdpClient.EnableBroadcast = true;
         _BroadCastAddress = new IPEndPoint(IPAddress.Parse(broadcastAddress), targetPort);
         _RemoteUdpEndPoint = new IPEndPoint(IPAddress.Parse(broadcastAddress), targetPort);
      }

      public void SendCommand(string command) {
         byte[] btarrRequest = Encoding.ASCII.GetBytes(command);
         _UdpClient.Send(btarrRequest, btarrRequest.Length, _BroadCastAddress);
      }

      public async Task<byte[]> ReceiveDataAsync() {
         var client = new TcpClient();
         await client.ConnectAsync(_RemoteTcpEndPoint.Address, _RemoteTcpEndPoint.Port);
         NetworkStream stream = client.GetStream();
         List<byte> data = new List<byte>();
         byte[] buffer = new byte[1];

         while(stream.DataAvailable) {
            stream.Read(buffer, 0, buffer.Length);
            data.AddRange(buffer);
         }
         client.Close();

         return data.ToArray();
      }

      public void GetTcpSettings() {
         byte[] btarrRequest = Encoding.ASCII.GetBytes(_SettingsRequest);
         _UdpClient.Send(btarrRequest, btarrRequest.Length, _BroadCastAddress);
         byte[] btarrResponse = _UdpClient.Receive(ref _RemoteUdpEndPoint);
         string strResponse = Encoding.ASCII.GetString(btarrResponse);
         string[] ipAdress = strResponse.Split(':');

         _RemoteTcpEndPoint = new IPEndPoint(IPAddress.Parse(ipAdress[0]), Convert.ToInt32(ipAdress[1]));
      }
   }
}