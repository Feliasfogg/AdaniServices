using System.Threading.Tasks;

namespace CoreLib.Listeners {
   public interface IListener {
      Task ListenUdpAsync();
      Task ListenTcpAsync();
      void ListenUdp();
      void ListenTcp();
   }
}