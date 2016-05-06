using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Entity;

namespace CoreLib.Commands {
   public class EditUserCommand: ServiceCommand {
      public UserInfo Info { get; set; }
   }
}
