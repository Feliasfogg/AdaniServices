using System.Runtime.Serialization;

namespace CoreLib.Commands.Common {
   //Прототип базовой команды
   [DataContract]
   public class ServiceCommand {
      [DataMember]
      public CommandActions Command { get; set; }

      [DataMember]
      public string SessionKey { get; set; }
   }
}