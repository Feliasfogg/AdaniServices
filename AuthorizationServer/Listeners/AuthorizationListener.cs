using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CoreLib;
using CoreLib.Commands;
using CoreLib.Commands.Authorization;
using CoreLib.Commands.Common;
using CoreLib.Encryption;
using CoreLib.Entity;
using CoreLib.Helpers;
using CoreLib.Serialization;

namespace AuthorizationServer.Listeners {
   public class AuthorizationListener : CommandListener {
      public AuthorizationListener(int listenPort, IPEndPoint localTcpEp) : base(listenPort, localTcpEp) {
      }

      protected override void Parse(byte[] data) {
         string strData = Encoding.ASCII.GetString(data);
         if(strData == "GET SETTINGS") {
            base.SendTcpSettings();
         }
         else {
            string publicKey = strData.Substring(strData.Length - 8);
            string hash = Encrypter.GeneratePasswordHash(publicKey);
            strData = strData.Substring(0, strData.Length - 8);
            string decryptXml = Encrypter.Decrypt(strData, hash);

            var xml = new XmlDocument();
            xml.LoadXml(decryptXml);
            XmlNodeList nodeList = xml.GetElementsByTagName("Command");
            var xmlNode = nodeList.Item(0);
            switch(xmlNode.InnerText) {
            case "Authorization":
               Authorize(decryptXml);
               break;
            case "GetUserInfo":
               GetUserInfo(decryptXml);
               break;
            case "EditUser":
               EditUserInfo(decryptXml);
               break;
            case "DeleteUser":
               DeleteUser(decryptXml);
               break;
            case "AddUser":
               AddUser(decryptXml);
               break;
            default:
               break;
            }
         }
      }


      private void Authorize(string xml) {
         try {
            var deserializer = new XmlSerializer<AuthorizationCommand>();
            var command = deserializer.Deserialize(xml);
            using(var provider = new EntityProvider()) {
               User user = provider.GetUserByCredentials(command.Login, command.Password);
               if(user != null) {
                  string sessionKey = provider.CreateSessionKey(user);
                  SendResponse(sessionKey);
               }
               else {
                  throw new Exception();
               }
            }
         }
         catch(Exception ex) {
            SendResponse("error");
         }
      }

      private void GetUserInfo(string xml) {
         try {
            var deserializer = new XmlSerializer<ServiceCommand>();
            var command = deserializer.Deserialize(xml);
            using(var provider = new EntityProvider()) {
               User user = provider.GetUserByKey(command.SessionKey);
               if(user == null) {
                  throw new Exception();
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
         catch(Exception ex) {
            SendResponse("error");
         }
      }

      private void EditUserInfo(string xml) {
         try {
            var deserializer = new XmlSerializer<UserCommand>();
            var command = deserializer.Deserialize(xml);
            using(var provider = new EntityProvider()) {
               User user = provider.GetUserById(command.Info.Id);
               if(user == null) {
                  throw new Exception();
               }
               user.Login = command.Info.Login;
               user.Password = command.Info.Password;
               user.AccessLevel = command.Info.AccessLevel;
               user.Name = command.Info.Name;
            }
            SendResponse("ok");
         }
         catch(Exception ex) {
            SendResponse("error");
         }
      }

      private void DeleteUser(string xml) {
         try {
            var deserializer = new XmlSerializer<DeleteUserCommand>();
            var command = deserializer.Deserialize(xml);
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
         catch(Exception ex) {
            SendResponse("error");
         }
      }

      private void AddUser(string xml) {
         try {
            var deserializer = new XmlSerializer<UserCommand>();
            var command = deserializer.Deserialize(xml);
            using(var provider = new EntityProvider()) {
               var user = new User() {
                  Login = command.Info.Login,
                  Password = command.Info.Password,
                  AccessLevel = command.Info.AccessLevel,
                  Name = command.Info.Name
               };
               provider.Users.Add(user);
            }
            SendResponse("ok");
         }
         catch(Exception ex) {
            SendResponse("error");
         }
      }
   }
}