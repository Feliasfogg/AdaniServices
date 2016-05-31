using System.Runtime.Serialization;

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
      WriteLog,
      [EnumMember]
      ExportDataBase
   }
}