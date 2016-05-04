using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Helpers {
   public interface IListener {
      void Listen();
      Task ListenAsync();
   }
}