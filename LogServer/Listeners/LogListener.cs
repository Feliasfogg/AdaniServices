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

namespace LogServer.Listeners
{
    public class LogListener : CommandListener
    {
        public LogListener(int listenPort, IPEndPoint localTcpEp) : base(listenPort, localTcpEp){
        }
        protected override void Parse(byte[] data)
        {
            string comname = "save-to-log";
            string decryptXml = Encoding.ASCII.GetString(Encrypter.DecryptData(data));
            var xml = new XmlDocument();
            xml.LoadXml(decryptXml);
            XmlNodeList nodeList = xml.GetElementsByTagName("Command");
            if (xml.GetElementsByTagName("Command").ToString()==comname)
            {
                var xmlNode = nodeList.Item(0);
            }
        }
        public void WriteLog()
        {
            string path=System.IO.Directory.GetCurrentDirectory().ToString()+@"logger.txt";
            byte[] txtbuff = new byte[4096];
            FileStream str = new FileStream("path", FileMode.Append, FileAccess.Write);
            str.WriteAsync(txtbuff, 0, (int)str.Length);
        }
    }
}
