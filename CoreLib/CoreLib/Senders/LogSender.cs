using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CoreLib.Commands.Common;
using CoreLib.Commands.Log;
using CoreLib.Serialization;

namespace CoreLib.Senders
{
    public class LogSender: CommandSender
    {
        public LogSender(IPAddress broadcastAddress, int targetPort) : base(broadcastAddress, targetPort){
        }
        public void SendException(Exception ex, string sessionkey)
        {
            if (ex.TargetSite.DeclaringType != null)
            {
                string fullText = $"[{DateTime.Now:dd.MM.yyy HH:mm:ss}] [{ex.Message}]";
                var command= new LogCommand()
                {
                    Command = CommandActions.WriteLog,
                    Message = fullText,SessionKey = sessionkey
                };
                var xml = XmlSerializer<LogCommand>.SerializeToXmlString(command);
                SendBroadcastCommand(xml);
            }
        }
        public void SendString(String message, string sessionkey)
        {
            string mess = $"[{DateTime.Now:dd.MM.yyy HH:mm:ss}] [{message}]";
            var command = new LogCommand()
            {
                Command = CommandActions.WriteLog,
                Message = mess,SessionKey = sessionkey
            };
            var xml = XmlSerializer<LogCommand>.SerializeToXmlString(command);
            SendBroadcastCommand(xml);
        }
    }
}
