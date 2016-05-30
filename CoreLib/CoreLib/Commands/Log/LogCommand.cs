using System.Runtime.Serialization;
using CoreLib.Commands.Common;

namespace CoreLib.Commands.Log {
   [DataContract]
   public class LogCommand : ServiceCommand {
      [DataMember]
      public string Message { get; set; }
   }
}