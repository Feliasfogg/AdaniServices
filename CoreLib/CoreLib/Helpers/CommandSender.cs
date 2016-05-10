using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands;
using CoreLib.Encryption;
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
      public CommandSender(IPAddress broadcastAddress, int targetPort) {
         _UdpClient = new UdpClient();
         _UdpClient.EnableBroadcast = true;
         _BroadCastAddress = new IPEndPoint(broadcastAddress, targetPort);
         _RemoteUdpEndPoint = new IPEndPoint(broadcastAddress, targetPort);
      }

      public void SendUdpCommand(string command) {
         command = CreateEncryptedCommand(command);
         byte[] btarrRequest = Encoding.ASCII.GetBytes(command);
         _UdpClient.Send(btarrRequest, btarrRequest.Length, _BroadCastAddress);
      }

      public void SendTcpCommand(string command) {
         var tcpClient = new TcpClient();
         tcpClient.Connect(_RemoteTcpEndPoint);
         byte[] bytes = Encoding.ASCII.GetBytes(command);
         using(NetworkStream stream = tcpClient.GetStream()) {
            stream.Write(bytes, 0, bytes.Length);
         }
      }

      private string CreateEncryptedCommand(string strCommand) {
         string password = Encrypter.CreatePassword(8);
         string encryptCommand = Encrypter.Encrypt(strCommand, password);
         encryptCommand += password;
         return encryptCommand;
      }

      public byte[] ReceiveData() {
         var tcpClient = new TcpClient();
         tcpClient.Connect(_RemoteTcpEndPoint);
         List<byte> data = new List<byte>();
         byte[] buffer = new byte[1];
         using (NetworkStream stream = tcpClient.GetStream()) {
            while(true) {
               stream.Read(buffer, 0, buffer.Length);
               data.AddRange(buffer);
               //если данные в стриме закончились прерываем цикл
               if(!stream.DataAvailable) {
                  break;
               }
            }
         }
         tcpClient.Close();
         //расшифровка данных
         string encryptedString = Encoding.ASCII.GetString(data.ToArray());
         string pass = encryptedString.Substring(encryptedString.Length - 8);
         encryptedString = encryptedString.Substring(0, encryptedString.Length - 8);
         string decryptString = Encrypter.Decrypt(encryptedString, pass);

         return Encoding.ASCII.GetBytes(decryptString);
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