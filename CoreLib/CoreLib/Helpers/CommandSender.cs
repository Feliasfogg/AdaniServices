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
      private IPEndPoint _BroadCastAddress;
      private IPEndPoint _RemoteUdpEndPoint;
      private IPEndPoint _RemoteTcpEndPoint;
      private IPEndPoint _LocalTcpEp;

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

      public void SendBroadcastCommand(string command) {
         command = CreateEncryptedCommand(command);
         byte[] bytes = Encoding.ASCII.GetBytes(command);
         _UdpClient.Send(bytes, bytes.Length, _BroadCastAddress);
      }

      public void SendTcpCommand(string command) {
         var tcpClient = new TcpClient();
         tcpClient.Connect(_RemoteTcpEndPoint);

         _LocalTcpEp = (IPEndPoint)tcpClient.Client.LocalEndPoint;

         command = CreateEncryptedCommand(command);
         byte[] bytes = Encoding.ASCII.GetBytes(command);
         using(NetworkStream stream = tcpClient.GetStream()) {
            stream.Write(bytes, 0, bytes.Length);
         }
         tcpClient.Close();
      }

      public byte[] ReceiveData() {
         var tcpListner = new TcpListener(_LocalTcpEp);
         tcpListner.Start();
         var tcpClient = tcpListner.AcceptTcpClient();

         List<byte> data = new List<byte>();
         byte[] buffer = new byte[1];
         using(NetworkStream stream = tcpClient.GetStream()) {
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
         tcpListner.Stop();

         //расшифровка данных
         return DecryptData(data.ToArray());
      }


      public void GetTcpSettings() {
         const string settings = "GET SETTINGS";
         byte[] btarrRequest = Encoding.ASCII.GetBytes(settings);
         _UdpClient.Send(btarrRequest, btarrRequest.Length, _BroadCastAddress);
         byte[] btarrResponse = _UdpClient.Receive(ref _RemoteUdpEndPoint);
         string strResponse = Encoding.ASCII.GetString(btarrResponse);
         string[] ipAdress = strResponse.Split(':');

         _RemoteTcpEndPoint = new IPEndPoint(IPAddress.Parse(ipAdress[0]), Convert.ToInt32(ipAdress[1]));
      }

      private string CreateEncryptedCommand(string strCommand) {
         string publicKey = Encrypter.GeneratePassword(8);
         string hash = Encrypter.GeneratePasswordHash(publicKey);
         string encryptCommand = Encrypter.Encrypt(strCommand, hash);
         encryptCommand += publicKey;
         return encryptCommand;
      }
      private byte[] DecryptData(byte[] data) {
         string encryptedString = Encoding.ASCII.GetString(data);
         string publicKey = encryptedString.Substring(encryptedString.Length - 8);
         encryptedString = encryptedString.Substring(0, encryptedString.Length - 8);
         string hash = Encrypter.GeneratePasswordHash(publicKey);
         string decryptString = Encrypter.Decrypt(encryptedString, hash);
         return Encoding.ASCII.GetBytes(decryptString);
      }
   }
}