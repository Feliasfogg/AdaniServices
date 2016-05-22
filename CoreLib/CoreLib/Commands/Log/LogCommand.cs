using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands.Common;

namespace CoreLib.Commands.Log
{
    [DataContract]
    public class LogCommand: ServiceCommand
    {
        [DataMember]
        public string Message { get; set; }    
    }
}
