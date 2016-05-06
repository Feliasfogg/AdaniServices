using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CoreLib.Commands;
using CoreLib.Commands.Authorization;
using CoreLib.Commands.Common;
using CoreLib.Entity;
using CoreLib.Helpers;
using CoreLib.Serialization;

namespace AuthorizationServer.Listeners {
   public class AuthorizationListener : CommandListener {
      public AuthorizationListener(int listenPort, IPEndPoint localTcpEp) : base(listenPort, localTcpEp) {
      }

      protected override async Task Parse(byte[] data) {
         string strData = Encoding.ASCII.GetString(data);
         if(strData == "GET SETTINGS") {
            base.SendTcpSettings();
         }
         else {
            var xml = new XmlDocument();
            xml.LoadXml(strData);
            XmlNodeList nodeList = xml.GetElementsByTagName("Command");
            var xmlNode = nodeList.Item(0);
            var responser = new Responser(_LocalTcpEp);
            switch(xmlNode.InnerText) {
            case "Authorization":
               Authorize(data);
               break;
            case "AuthorizationInfo":
               SendAuthorizationInfo(data);
               break;
            case "EditUser":
               EditUserInfo(data);
               break;
            case "DeleteUser":
               DeleteUser(data);
               break;
            default:
               break;
            }
         }
      }


      private void Authorize(byte[] data) {
         var deserializer = new XmlSerializer<AuthorizationCommand>();
         var command = deserializer.Deserialize(data);
         using(var provider = new EntityProvider()) {
            User user = provider.GetUserByCredentials(command.Login, command.Password);
            if(user != null) {
               string sessionKey = provider.CreateSessionKey(user);
               SendResponse(sessionKey);
            }
            else {
               SendResponse("error");
            }
         }
      }

      private void SendAuthorizationInfo(byte[] data) {
         var deserializer = new XmlSerializer<ServiceCommand>();
         var command = deserializer.Deserialize(data);
         using(var provider = new EntityProvider()) {
            User user = provider.GetUserByKey(command.SessionKey);
            if(user == null) {
               SendResponse("error");
               return;
            }
            var userEntity = new UserInfo() {
               Id = user.Id,
               Login = user.Login,
               Name = user.Name,
               Password = user.Password,
               AccessLevel = user.AccessLevel
            };

            var serializer = new XmlSerializer<UserInfo>();
            byte[] btarr = serializer.SerializeToBytes(userEntity);

            SendResponse(btarr);
         }
      }

      private void EditUserInfo(byte[] data) {
         var deserializer = new XmlSerializer<EditUserCommand>();
         var command = deserializer.Deserialize(data);
         using(var provider = new EntityProvider()) {
            User user = provider.GetUserById(command.Info.Id);
            if(user == null) {
               SendResponse("error");
               return;
            }
            user.Login = command.Info.Login;
            user.Password = command.Info.Password;
            user.AccessLevel = command.Info.AccessLevel;
            user.Name = command.Info.Name;
         }
         SendResponse("ok");
      }

      private void DeleteUser(byte[] data) {
         var deserializer = new XmlSerializer<DeleteUserCommand>();
         var command = deserializer.Deserialize(data);
         using(var provider = new EntityProvider()) {
            bool result = provider.DeleteUser(command.UserId);
            if(result) {
               SendResponse("ok");
            }
            else {
               SendResponse("error");
            }
         }
      }
   }
}