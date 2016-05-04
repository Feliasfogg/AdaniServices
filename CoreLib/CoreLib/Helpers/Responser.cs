using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Commands;
using CoreLib.Entity;
using CoreLib.Serialization;

namespace CoreLib.Helpers {
   public class Responser {
      private IPEndPoint _TcpEp;

      public Responser(IPEndPoint ep) {
         _TcpEp = ep;
      }

      public void CommandExecute(ServiceCommand command) {
         switch(command.Command) {
         case CommandActions.AuthorizationInfo:
            SendAuthorizationInfo(command);
            break;
         case CommandActions.Authorization:
            Authorize(command);
            break;
         }
      }

      private void Authorize(ServiceCommand command) {
         var authCommand = (AuthorizationCommand)command;
         using(var provider = new EntityProvider()) {
            User user = provider.GetUserByCredentials(authCommand.Login, authCommand.Password);
            if(user != null) {
               string sessionKey = provider.CreateSessionKey(user);
               SendResponse(Encoding.ASCII.GetBytes(sessionKey));
            }
            else {
               SendResponse(Encoding.ASCII.GetBytes("error"));
            }
         }
      }

      private void SendAuthorizationInfo(ServiceCommand command) {
         using(var provider = new EntityProvider()) {
            User user = provider.GetUserByKey(command.SessionKey);
            if(user == null) {
               return;
            }
            var userEntity = new UserEntity() {
               Id = user.Id,
               Login = user.Login,
               Name = user.Name,
               Password = user.Password,
               AccessLevel = user.AccessLevel
            };

            var serializer = new XmlSerialization<UserEntity>();
            Stream dataStream = serializer.Serialize(userEntity);
            byte[] btarr = new byte[dataStream.Length];
            dataStream.Read(btarr, 0, btarr.Length);
            SendResponse(btarr);
         }
      }

      private void SendResponse(byte[] btarr) {
         var listener = new TcpListener(_TcpEp);
         listener.Start();
         TcpClient client = listener.AcceptTcpClient();
         using(NetworkStream networkStream = client.GetStream()) {
            networkStream.Write(btarr, 0, btarr.Length);
         }
         listener.Stop();
      }
   }
}