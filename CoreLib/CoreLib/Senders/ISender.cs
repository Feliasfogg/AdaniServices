namespace CoreLib.Senders {
   public interface ISender {
      void SendBroadcastCommand(string command);
      void SendTcpCommand(string command);
      byte[] ReceiveData();
   }
}