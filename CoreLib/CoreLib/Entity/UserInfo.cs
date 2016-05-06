using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Entity {

   [DataContract]
   public class UserInfo {
      [DataMember]
      public int Id { get; set; }
      [DataMember]
      public string Name { get; set; }
      [DataMember]
      public string Login { get; set; }
      [DataMember]
      public string Password { get; set; }
      [DataMember]
      public long AccessLevel { get; set; }
      [DataMember]
      public SessionKey SessionKey { get; set; }
   }
}
