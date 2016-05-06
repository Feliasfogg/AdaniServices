using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands.Common;

namespace CoreLib.Commands.Settings {
   public class DeviceSettingsCommand:ServiceCommand {
      public int DeviceId { get; set; }
      public int GroupId { get; set; }
   }
}
