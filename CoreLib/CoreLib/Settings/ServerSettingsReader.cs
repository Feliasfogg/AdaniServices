using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CoreLib.Settings {
   /*Example of xml
       <?xml version="1.0" encoding="utf-8"?>
         <settings>
	         <tcp_address>
		         <ip>127.0.0.1</ip>
		         <port>11000</port>
	         </tcp_address>
	         <udp_port>4444</udp_port>
	         <data_folder></data_folder>
         </settings>   */

   public class ServerSettingsReader {
      public static ServerSettings ReadXml() {
         var settings = new ServerSettings();

         var xml = new XmlDocument();
         string path = $"{AppDomain.CurrentDomain.BaseDirectory}settings.xml";
         var xmlReader = new XmlTextReader(path);
         xml.Load(xmlReader);

         XmlNodeList tcpAddress = xml.GetElementsByTagName("tcp_address");
         string tcpIp = tcpAddress.Item(0)["ip"].InnerText;
         string tcpPort = tcpAddress.Item(0)["port"].InnerText;

         XmlNodeList udpPort = xml.GetElementsByTagName("udp_port");
         string strUdpPort = udpPort.Item(0).InnerText;
         XmlNodeList dataFolder = xml.GetElementsByTagName("data_folder");
         string strDataFolder = dataFolder.Item(0).InnerText;

         settings.TcpEp = new IPEndPoint(IPAddress.Parse(tcpIp), Convert.ToInt32(tcpPort));
         settings.UdpPort = Convert.ToInt32(strUdpPort);
         settings.DataFolderPath = strDataFolder;

         return settings;
      }
   }
}