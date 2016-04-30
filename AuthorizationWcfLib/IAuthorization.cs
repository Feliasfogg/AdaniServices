using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands;

namespace AuthorizationWcfLib {
   [ServiceContract]
   public interface IAuthorization {
      [OperationContract]
      string GetSessionKey(AuthorizationCommand command);

      [OperationContract]
      string GetTcpSettings();
   }
}
