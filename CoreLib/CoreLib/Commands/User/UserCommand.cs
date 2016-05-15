using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands;
using CoreLib.Commands.Common;
using CoreLib.Entity;
using Microsoft.SqlServer.Server;

namespace CoreLib.Commands.User {
   [DataContract]
   public class UserCommand : ServiceCommand {
      [DataMember]
      public Entity.User User { get; set; }
      [DataMember]
      public int UserId { get; set; }

      [DataMember]
      public string Login { get; set; }

      [DataMember]
      public string Password { get; set; }
   }
}