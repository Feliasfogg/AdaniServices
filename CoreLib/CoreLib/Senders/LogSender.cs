using System;
using System.Net;
using CoreLib.Commands.Common;
using CoreLib.Commands.Log;
using CoreLib.Serialization;

namespace CoreLib.Senders {
   public class LogSender : CommandSender {
      public LogSender(IPAddress broadcastAddress, int targetPort) : base(broadcastAddress, targetPort) {
      }

      public void SendException(Exception ex, string sessionKey = null) {
         if(ex.TargetSite.DeclaringType != null) {
            string fullText = $"[{DateTime.Now:dd.MM.yyy HH:mm:ss}] [{ex.Message}]";
            var command = new LogCommand() {
               Command = CommandActions.WriteLog,
               Message = fullText
            };
            if(sessionKey != null) {
               command.SessionKey = sessionKey;
            }
            var xml = XmlSerializer<LogCommand>.SerializeToXmlString(command);
            SendBroadcastCommand(xml);
         }
      }

      public void SendString(String message, string sessionkey = null) {
         string mess = $"[{DateTime.Now:dd.MM.yyy HH:mm:ss}] [{message}]";

         LogCommand command = new LogCommand() {
            Command = CommandActions.WriteLog,
            Message = mess
         };
         if(sessionkey != null) {
            command.SessionKey = sessionkey;
         }
         var xml = XmlSerializer<LogCommand>.SerializeToXmlString(command);
         SendBroadcastCommand(xml);
      }
   }
}