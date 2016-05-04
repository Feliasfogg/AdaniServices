using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CoreLib.Commands;
using CoreLib.Entity;
using CoreLib.Helpers;
using CoreLib.Serialization;

namespace AuthorizationServer.Listeners {
   public class AuthorizationListener : CommandListener {
      public AuthorizationListener(int listenPort, IPEndPoint localTcpEp) : base(listenPort, localTcpEp) {
      }

      protected override void Parse(byte[] data) {
         string strData = Encoding.ASCII.GetString(data);
         if (strData == "GET SETTINGS") {
            base.SendTcpSettings();
         } else {
            var xml = new XmlDocument();
            xml.LoadXml(strData);
            XmlNodeList nodeList = xml.GetElementsByTagName("Command");
            var xmlNode = nodeList.Item(0);
            var responser = new Responser(_LocalTcpEp);
            switch (xmlNode.InnerText) {
               case "Authorization": {
                     var deserializer = new XmlSerialization<AuthorizationCommand>();
                     var command = deserializer.Deserialize(new MemoryStream(data));
                     CommandExecute(command);
                  }
                  break;
               case "AuthorizationInfo": {
                     var deserializer = new XmlSerialization<ServiceCommand>();
                     var command = deserializer.Deserialize(new MemoryStream(data));
                     CommandExecute(command);
                  }
                  break;
               default:
                  break;
            }
         }
      }


      protected override void CommandExecute(ServiceCommand command) {
         switch (command.Command) {
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
         using (var provider = new EntityProvider()) {
            User user = provider.GetUserByCredentials(authCommand.Login, authCommand.Password);
            var responser = new Responser(_LocalTcpEp);
            if (user != null) {
               string sessionKey = provider.CreateSessionKey(user);
               responser.SendResponse(Encoding.ASCII.GetBytes(sessionKey));
            } else {
               responser.SendResponse(Encoding.ASCII.GetBytes("error"));
            }
         }
      }

      private void SendAuthorizationInfo(ServiceCommand command) {
         using (var provider = new EntityProvider()) {
            User user = provider.GetUserByKey(command.SessionKey);
            if (user == null) {
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
            byte[] btarr = serializer.SerializeToBytes(userEntity);
            var responser = new Responser(_LocalTcpEp);
            responser.SendResponse(btarr);
         }
      }
   }
}
