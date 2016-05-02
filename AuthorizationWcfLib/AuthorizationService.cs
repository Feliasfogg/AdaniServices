using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthorizationWcfLib.Data;
using CoreLib.Commands;
using CoreLib.Entity;

namespace AuthorizationWcfLib {
   public class AuthorizationService : IAuthorization {
      public string Authorization(AuthorizationCommand command) {
         string sessionKey = String.Empty;
         using(var provider = new EntityProvider()) {
            User user = provider.GetUserByCredentials(command.Login, command.Password);
            if(user != null) {
               sessionKey = provider.CreateSessionKey(user);
            }
         }
         return sessionKey;
      }
   }
}