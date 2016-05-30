using CoreLib.Commands.Common;
using CoreLib.Entity;

namespace CoreLib.Commands.Settings {
   public class SettingsCommand : ServiceCommand {
      public Device Device { get; set; }
      public int DeviceId { get; set; }
      public int GroupId { get; set; }
   }
}