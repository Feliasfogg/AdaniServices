using CoreLib.Encryption;
using CoreLib.Listeners;
using System.Net;
using System.Text;
using System.Xml;
using CoreLib.Commands.Log;
using CoreLib.Helpers;
using CoreLib.Serialization;

namespace LogServer.Listeners {
   public class LogListener : CommandListener {
      private string _FolderPath;

      public LogListener(int listenPort, IPEndPoint localTcpEp, string folderPath) : base(listenPort, localTcpEp) {
         _FolderPath = folderPath;
      }

      protected override void Parse(byte[] data) {
         //string comname = "WriteLog";
         string decryptXml = Encoding.ASCII.GetString(Encrypter.DecryptData(data));
         var xml = new XmlDocument();
         xml.LoadXml(decryptXml);
         XmlNodeList nodeList = xml.GetElementsByTagName("Command");
         var xmlNode = nodeList.Item(0);
         if(xmlNode.InnerText == "WriteLog") {
            WrtieLog(decryptXml);
         }
      }

      private void WrtieLog(string xml) {
         var command = XmlSerializer<LogCommand>.Deserialize(xml);
         string fullMessage;
         if(command.SessionKey != null) {
            fullMessage = $"{command.Message} SessionKey: {command.SessionKey}";
         }
         else {
            fullMessage = $"{command.Message}";
         }
         LogHelper.Write(fullMessage, _FolderPath);
      }
   }
}