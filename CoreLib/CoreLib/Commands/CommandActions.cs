using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Commands {
   [DataContract]
   public enum CommandActions {
      [EnumMember]
      Authorization,

      [EnumMember]
      AddUser,

      [EnumMember]
      EditUser,

      [EnumMember]
      DeleteUser,

      [EnumMember]
      GetUserInfo,

      [EnumMember]
      GetSettings
   }
}