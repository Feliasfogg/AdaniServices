using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands.Common;
using CoreLib.Entity;

namespace CoreLib.Commands.Authorization {
   public class UserCommand: ServiceCommand {
      public User User { get; set; }
   }
}
