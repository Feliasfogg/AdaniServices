using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreLib.Encryption;
using Timer = System.Timers.Timer;

namespace CoreLib.Listeners {
   /// <summary>
   /// class provides easy methods for listen local udp port and recives tcp settings for client
   /// </summary>
   public abstract class CommandListener : IListener {
      protected IPEndPoint _RemoteUdpEp;
      protected IPEndPoint _RemoteTcpEp;
      protected IPEndPoint _LocalTcpEp;
      protected int _ListenPort;
      protected TcpListener _TcpListener;

      /// <summary>
      /// Создает объект слушателя, который слушает Udp и Tcp протоколы
      /// </summary>
      /// <param name="listenPort">прослушиваемый порт</param>
      /// <param name="localTcpEp">локальная оконечная точка для Tcp соединения</param>
      public CommandListener(int listenPort, IPEndPoint localTcpEp) {
         _ListenPort = listenPort;
         _LocalTcpEp = localTcpEp;
         _RemoteUdpEp = new IPEndPoint(IPAddress.Any, 1111);
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
         //мы слушаем TcpListner входящие tcp соединения, на стороне клиента тоже есть
         //tcplistner, который принимает входящие соединения от сервера
         while(true) {
            TcpClient client = _TcpListener.AcceptTcpClient();
            _RemoteTcpEp = (IPEndPoint)client.Client.RemoteEndPoint;

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
      }

      public void ListenUdp() {
         //мы слушаем TcpListner входящие tcp соединения
         UdpClient client;
         while(true) {
            client = new UdpClient(_ListenPort);
            byte[] data = client.Receive(ref _RemoteUdpEp);
            client.Close();
            Task.Run(() => Parse(data));
         }
      }

      //метод парсит получаемые данные, его имплементация расположена в наследниках
      protected abstract void Parse(byte[] data);

      protected void SendTcpSettings() {
         var strAddress = _LocalTcpEp.ToString();
         byte[] btarr = Encoding.ASCII.GetBytes(strAddress);

         var client = new UdpClient();
         client.Connect(_RemoteUdpEp);
         client.Send(btarr, btarr.Length);
      }

      //метод для посылки ответа запросившему клиенту.
      protected void SendResponse(byte[] data) {
         //шифрование данных
         byte[] encryptData = Encrypter.EncryptData(data);

         TcpClient client = new TcpClient();

         //ожидаем 30c пока не стороне клиента появится принимающий сокет,
         //если не появился return
         double timeOut = 10000;
         while(!client.Connected) {
            try {
               client.Connect(_RemoteTcpEp);
            }
            catch(SocketException ex) {
               Thread.Sleep(1000);
               timeOut -= 1000;
               if(timeOut <= 0) {
                  //return сделан вместо выброса исключения потому что если принимающий сокет так и не проявился
                  //мы будем пытаться циклично передать текст исключения сокету.
                  return;
               }
               continue;
            }
         }

         using(NetworkStream networkStream = client.GetStream()) {
            networkStream.Write(encryptData, 0, encryptData.Length);
         }
         client.Close();
      }

      protected void SendResponse(string str) {
         SendResponse(Encoding.ASCII.GetBytes(str));
      }
   }
}