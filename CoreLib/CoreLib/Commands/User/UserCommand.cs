using System.Runtime.Serialization;
using CoreLib.Commands.Common;

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
      public string PasswordHash { get; set; }
   }
}