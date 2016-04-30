using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands;

namespace AuthorizationWcfLib {
   public class AuthorizationService:IAuthorization {
      public string GetSessionKey(AuthorizationCommand command) {
         return "sessionkey";
      }

      public string GetTcpSettings() {
         return "settings";
      }
   }
}
