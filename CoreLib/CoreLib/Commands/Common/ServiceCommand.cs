using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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