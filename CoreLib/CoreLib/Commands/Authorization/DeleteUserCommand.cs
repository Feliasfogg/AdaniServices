using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands.Common;

namespace CoreLib.Commands.Authorization {
   public class DeleteUserCommand:ServiceCommand {
      public int UserId { get; set; }
   }
}
