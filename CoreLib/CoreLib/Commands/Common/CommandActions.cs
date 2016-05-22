using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Commands.Common {
   //Перечисление с действиями команд
   [DataContract]
   public enum CommandActions {
      [EnumMember]
      Authorization,

      [EnumMember]
      AddUser,

      [EnumMember]
      EditUser,

      [EnumMember]
      RemoveUser,

      [EnumMember]
      GetUser,

      [EnumMember]
      GetDevice,

      [EnumMember]
      AddDevice,

      [EnumMember]
      EditDevice,

      [EnumMember]
      RemoveDevice,

      [EnumMember]
      WriteLog
    }
}