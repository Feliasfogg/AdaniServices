using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Helpers {
   /// <summary>
   /// class provides easy methods for listen local udp port and recives tcp settings for client
   /// </summary>
   public class UdpHelper {
      private UdpClient _UdpListener;
      private IPEndPoint _RemoteEndPoint;
      private IPEndPoint _SettingsTcpEndPoint;

      /// <summary>
      /// create instance of UdpHelper
      /// </summary>
      /// <param name="listenPort">local listen port</param>
      /// <param name="settingsTcpEndPoint">settings of TCP endpoint</param>
      public UdpHelper(int listenPort, IPEndPoint settingsTcpEndPoint) {
         _RemoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
         _UdpListener = new UdpClient(_RemoteEndPoint);
         _SettingsTcpEndPoint = settingsTcpEndPoint;
      }

      public Task ListenAsync() {
         return Task.Run(() => Listen());
      }

      public void Listen() {
         while(true) {
            byte[] data = _UdpListener.Receive(ref _RemoteEndPoint);
            if(Parse(data)) {
               SendSettings();
            }
         }
      }

      private bool Parse(byte[] data) {
         string result = Encoding.ASCII.GetString(data);
         return result == "GET SETTINGS";
      }

      private void SendSettings() {
         var strAddress = _SettingsTcpEndPoint.ToString();
         byte[] btarr = Encoding.ASCII.GetBytes(strAddress);
         _UdpListener.Connect(_RemoteEndPoint);
         _UdpListener.Send(btarr, btarr.Length);
      }
   }
}