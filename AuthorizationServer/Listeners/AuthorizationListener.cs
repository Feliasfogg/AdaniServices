using System;
using System.Net;
using System.Text;
using System.Xml;
using CoreLib.Commands.Common;
using CoreLib.Commands.User;
using CoreLib.Encryption;
using CoreLib.Entity;
using CoreLib.Listeners;
using CoreLib.Serialization;

namespace AuthorizationServer.Listeners {
   public class AuthorizationListener : CommandListener {
      public AuthorizationListener(int listenPort, IPEndPoint localTcpEp) : base(listenPort, localTcpEp) {
      }

      protected override void Parse(byte[] data) {
         string strData = Encoding.ASCII.GetString(data);
         if(strData == "GET SETTINGS") {
            SendTcpSettings();
         }
         else {
            //дешифровка
            string decryptXml = Encoding.ASCII.GetString(Encrypter.DecryptData(data));
            //парсинг результирующего xml
            var xml = new XmlDocument();
            xml.LoadXml(decryptXml);
            XmlNodeList nodeList = xml.GetElementsByTagName("Command");
            var xmlNode = nodeList.Item(0);
            //выбор команды для выполнения
            switch(xmlNode.InnerText) {
            case "Authorization":
               Authorize(decryptXml);
               break;
            case "GetUser":
               GetUser(decryptXml);
               break;
            case "EditUser":
               EditUser(decryptXml);
               break;
            case "RemoveUser":
               RemoveUser(decryptXml);
               break;
            case "AddUser":
               AddUser(decryptXml);
               break;
            default:
               break;
            }
         }
      }


      private void Authorize(string xmlCommand) {
         try {
            var command = XmlSerializer<UserCommand>.Deserialize(xmlCommand);
            string sessionKey;
            using(var provider = new EntityProvider()) {
               User user = provider.GetUserByCredentials(command.Login, command.PasswordHash);
               if(user != null) {
                  sessionKey = provider.CreateSessionKey(user);
               }
               else {
                  throw new Exception("No exist user");
               }
            }
            SendResponse(sessionKey);
         }
         catch(Exception ex) {
            SendResponse($"{ex.Message} in {nameof(Authorize)}");
         }
      }

      private void GetUser(string xmlCommand) {
         try {
            var command = XmlSerializer<ServiceCommand>.Deserialize(xmlCommand);
            string xmlUserInfo;
            using(var provider = new EntityProvider()) {
               User user = provider.GetUserByKey(command.SessionKey);
               if(user == null) {
                  throw new Exception("No exist user");
               }
               xmlUserInfo = XmlSerializer<User>.SerializeToXmlString(user);
            }
            SendResponse(xmlUserInfo);
         }
         catch(Exception ex) {
            SendResponse($"{ex.Message} in {nameof(GetUser)}");
         }
      }

      private void EditUser(string xmlCommand) {
         try {
            var command = XmlSerializer<UserCommand>.Deserialize(xmlCommand);
            using(var provider = new EntityProvider()) {
               User user = provider.GetUserById(command.User.Id);
               if(user == null) {
                  throw new Exception("No exist user");
               }
               //если сделать так user = command.User; то изменения в базу незапушутся
               user.AccessLevel = command.User.AccessLevel;
               user.Login = command.User.Login;
               user.PasswordHash = command.User.PasswordHash;
               user.Name = command.User.Name;
            }
            SendResponse("ok");
         }
         catch(Exception ex) {
            SendResponse($"{ex.Message} in {nameof(EditUser)}");
         }
      }

      private void RemoveUser(string xmlCommand) {
         try {
            var command = XmlSerializer<UserCommand>.Deserialize(xmlCommand);
            using(var provider = new EntityProvider()) {
               bool result = provider.RemoveUser(command.UserId);
               if(!result) {
                  throw new Exception("Cant delete user");
               }
            }
            SendResponse("ok");
         }
         catch(Exception ex) {
            SendResponse($"{ex.Message} in {nameof(RemoveUser)}");
         }
      }

      private void AddUser(string xmlCommand) {
         try {
            var command = XmlSerializer<UserCommand>.Deserialize(xmlCommand);
            using(var provider = new EntityProvider()) {
               bool result = provider.AddUser(command.User);
               if(!result) {
                  throw new Exception("Cant add user");
               }
            }
            SendResponse("ok");
         }
         catch(Exception ex) {
            SendResponse($"{ex.Message} in {nameof(AddUser)}");
         }
      }
   }
}