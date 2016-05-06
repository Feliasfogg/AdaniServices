using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands;
using CoreLib.Commands.Common;
using Microsoft.SqlServer.Server;

namespace CoreLib.Commands.Authorization {
   [DataContract]
   public class AuthorizationCommand : ServiceCommand {
      [DataMember]
      public string Login { get; set; }

      [DataMember]
      public string Password { get; set; }
   }
}