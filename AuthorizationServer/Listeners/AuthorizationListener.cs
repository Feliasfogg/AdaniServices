﻿using System;
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
            //дешифровка
            string publicKey = strData.Substring(strData.Length - 8);
            string hash = Encrypter.GeneratePasswordHash(publicKey);
            strData = strData.Substring(0, strData.Length - 8);
            string decryptXml = Encrypter.Decrypt(strData, hash);
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
            var command = XmlSerializer<AuthorizationCommand>.Deserialize(xml);
            string sessionKey;
            using(var provider = new EntityProvider()) {
               User user = provider.GetUserByCredentials(command.Login, command.Password);
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
            SendResponse(ex.Message);
         }
      }

      private void GetUserInfo(string xml) {
         try {
            var command = XmlSerializer<ServiceCommand>.Deserialize(xml);
            string xmlString;
            using (var provider = new EntityProvider()) {
               User user = provider.GetUserByKey(command.SessionKey);
               if(user == null) {
                  throw new Exception("No exist user");
               }
               xmlString = XmlSerializer<User>.SerializeToXmlString(user);

            }
            SendResponse(xmlString);
         } catch (Exception ex) {
            SendResponse(ex.Message);
         }
      }

      private void EditUserInfo(string xml) {
         try {
            var command = XmlSerializer<UserCommand>.Deserialize(xml);
            using(var provider = new EntityProvider()) {
               User user = provider.GetUserById(command.User.Id);
               if(user == null) {
                  throw new Exception("No exist user");
               }
               user.Login = command.User.Login;
               user.Password = command.User.Password;
               user.AccessLevel = command.User.AccessLevel;
               user.Name = command.User.Name;
            }
            SendResponse("ok");
         }
         catch(Exception ex) {
            SendResponse(ex.Message);
         }
      }

      private void DeleteUser(string xml) {
         try {
            var command = XmlSerializer<DeleteUserCommand>.Deserialize(xml);
            using(var provider = new EntityProvider()) {
               bool result = provider.DeleteUser(command.UserId);
               if(!result) {
                  throw new Exception("Cant delete user");
               }
            }
            SendResponse("ok");
         }
         catch(Exception ex) {
            SendResponse(ex.Message);
         }
      }

      private void AddUser(string xml) {
         try {
            var command = XmlSerializer<UserCommand>.Deserialize(xml);
            using(var provider = new EntityProvider()) {
               bool result = provider.AddUser(command.User);
               if(!result) {
                  throw new Exception("Cant add user");
               }
            }
            SendResponse("ok");
         }
         catch(Exception ex) {
            SendResponse(ex.Message);
         }
      }
   }
}