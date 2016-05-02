using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Entity {
   [DataContract]
   public class SessionKeyEntity {
      [DataMember]
      public int Id { get; set; }

      [DataMember]
      public string Key { get; set; }

      [DataMember]
      public System.DateTime ExpirationTime { get; set; }

      [DataMember]
      public User User { get; set; }
   }
}