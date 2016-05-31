using System.Runtime.Serialization;
using CoreLib.Commands.Common;
using CoreLib.Entity;

namespace CoreLib.Commands.Settings {
   [DataContract]
   public class SettingsCommand : ServiceCommand {
      [DataMember]
      public Device Device { get; set; }

      [DataMember]
      public int DeviceId { get; set; }

      [DataMember]
      public int GroupId { get; set; }
   }
}