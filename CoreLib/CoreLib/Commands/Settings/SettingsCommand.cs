using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands.Common;
using CoreLib.Entity;

namespace CoreLib.Commands.Settings {
   public class SettingsCommand:ServiceCommand {
      public Device Device { get; set; }
      public int DeviceId { get; set; }
      public int GroupId { get; set; }
   }
}
