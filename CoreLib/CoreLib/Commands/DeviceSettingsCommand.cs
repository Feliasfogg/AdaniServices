using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Commands {
   public class DeviceSettingsCommand:ServiceCommand {
      public int DeviceId { get; set; }
   }
}
