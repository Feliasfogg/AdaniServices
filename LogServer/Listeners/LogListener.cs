using CoreLib.Encryption;
using CoreLib.Listeners;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CoreLib.Commands.Log;
using CoreLib.Helpers;
using CoreLib.Serialization;

namespace LogServer.Listeners
{
    public class LogListener : CommandListener
    {
        public LogListener(int listenPort, IPEndPoint localTcpEp) : base(listenPort, localTcpEp){
        }
        protected override void Parse(byte[] data)
        {
            //string comname = "WriteLog";
            string decryptXml = Encoding.ASCII.GetString(Encrypter.DecryptData(data));
            var xml = new XmlDocument();
            xml.LoadXml(decryptXml);
            XmlNodeList nodeList = xml.GetElementsByTagName("Command");
            var xmlNode = nodeList.Item(0);
            if (xmlNode.InnerText == "WriteLog")
            {
                WrtieLog(decryptXml);
            }

        }
        private void WrtieLog(string xml)
        {
            var command = XmlSerializer<LogCommand>.Deserialize(xml);
            string fullmess = $"{command.Message} SessionKey: { command.SessionKey}";
            LogHelper.Write(fullmess);
        }
    }
}
