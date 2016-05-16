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

namespace CoreLib.Senders {
   public class CommandSender : ISender {
      private UdpClient _UdpClient;
      private IPEndPoint _BroadCastAddress;
      private IPEndPoint _RemoteUdpEndPoint;
      private IPEndPoint _RemoteTcpEndPoint;
      private IPEndPoint _LocalTcpEp;

      /// <summary>
      /// Создает объект посылателя. Посылатель может посылать как широковещательные сообщения так
      /// и сообщения на получаемый от сервера Tcp адрес
      /// </summary>
      /// <param name="broadcastAddress">широковещательный адрес сети</param>
      /// <param name="targetPort">порт на который посылаются широковещательные сообщения</param>
      public CommandSender(IPAddress broadcastAddress, int targetPort) {
         _UdpClient = new UdpClient();
         _UdpClient.EnableBroadcast = true;
         _BroadCastAddress = new IPEndPoint(broadcastAddress, targetPort);
         _RemoteUdpEndPoint = new IPEndPoint(broadcastAddress, targetPort);
      }
      //посылка широковещательной зашифрованной команды
      public void SendBroadcastCommand(string command) {
         byte[] bytes = Encrypter.EncryptData(command);
         _UdpClient.Send(bytes, bytes.Length, _BroadCastAddress);
      }
      //посылка зашифрованной команды по Tcp
      public void SendTcpCommand(string command) {
         var tcpClient = new TcpClient();
         tcpClient.Connect(_RemoteTcpEndPoint);

         _LocalTcpEp = (IPEndPoint)tcpClient.Client.LocalEndPoint;

         byte[] bytes = Encrypter.EncryptData(command);
         using(NetworkStream stream = tcpClient.GetStream()) {
            stream.Write(bytes, 0, bytes.Length);
         }
         tcpClient.Close();
      }
      //Получение данных от сервера.
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
         return Encrypter.DecryptData(data.ToArray());
      }

      //запрос Tcp адреса сервера
      public void GetTcpSettings() {
         const string settings = "GET SETTINGS";
         byte[] btarrRequest = Encoding.ASCII.GetBytes(settings);
         _UdpClient.Send(btarrRequest, btarrRequest.Length, _BroadCastAddress);
         byte[] btarrResponse = _UdpClient.Receive(ref _RemoteUdpEndPoint);
         string strResponse = Encoding.ASCII.GetString(btarrResponse);
         string[] ipAdress = strResponse.Split(':');

         _RemoteTcpEndPoint = new IPEndPoint(IPAddress.Parse(ipAdress[0]), Convert.ToInt32(ipAdress[1]));
      }
   }
}