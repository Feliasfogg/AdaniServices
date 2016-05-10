using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CoreLib.Commands;
using CoreLib.Encryption;
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
      protected TcpListener _TcpListener;

      /// <summary>
      /// create instance of UdpHelper
      /// </summary>
      /// <param name="listenPort">local listen port</param>
      /// <param name="localTcpEp">local service TCP endpoint</param>
      public CommandListener(int listenPort, IPEndPoint localTcpEp) {
         _ListenPort = listenPort;
         _LocalTcpEp = localTcpEp;
         _RemoteEndPoint = new IPEndPoint(IPAddress.Any, 1111);
         _TcpListener = new TcpListener(localTcpEp);
         _TcpListener.Start();
      }

      public Task ListenTcpAsync() {
         return Task.Run(() => ListenTcp());
      }
      public Task ListenUdpAsync() {
         return Task.Run(() => ListenUdp());
      }

      public void ListenTcp() {
         TcpClient client = _TcpListener.AcceptTcpClient();

         List<byte> data = new List<byte>();
         byte[] buffer = new byte[1];
         using(NetworkStream stream = client.GetStream()) {
            while(true) {
               stream.Read(buffer, 0, buffer.Length);
               data.AddRange(buffer);
               //если данные в стриме закончились прерываем цикл
               if(!stream.DataAvailable) {
                  break;
               }
            }
         }
         client.Close();

         Task.Run(() => Parse(data.ToArray()));
      }

      public void ListenUdp() {
         UdpClient client;
         while(true) {
            client = new UdpClient(_ListenPort);
            byte[] data = client.Receive(ref _RemoteEndPoint);
            client.Close();
            Task.Run(() => Parse(data));
         }
      }

      protected abstract void Parse(byte[] data);

      protected void SendTcpSettings() {
         var strAddress = _LocalTcpEp.ToString();
         byte[] btarr = Encoding.ASCII.GetBytes(strAddress);

         var client = new UdpClient();
         client.Connect(_RemoteEndPoint);
         client.Send(btarr, btarr.Length);
      }

      protected void SendResponse(byte[] bytes) {
         string data = Encoding.ASCII.GetString(bytes);
         string pass = Encrypter.CreatePassword(8);
         string encryptedString = Encrypter.Encrypt(data, pass);
         encryptedString += pass;

         bytes = Encoding.ASCII.GetBytes(encryptedString);
         TcpClient client = _TcpListener.AcceptTcpClient();
         using (NetworkStream networkStream = client.GetStream()) {
            networkStream.Write(bytes, 0, bytes.Length);
         }
      }
      protected void SendResponse(string str) {
         string pass = Encrypter.CreatePassword(8);
         string encryptedString = Encrypter.Encrypt(str, pass);
         encryptedString += pass;

         byte[] bytes = Encoding.ASCII.GetBytes(encryptedString);
         TcpClient client = _TcpListener.AcceptTcpClient();
         using (NetworkStream networkStream = client.GetStream()) {
            networkStream.Write(bytes, 0, bytes.Length);
         }
      }
   }
}