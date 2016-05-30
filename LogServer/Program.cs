using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DeviceSettingsServer.Listeners;
using CoreLib.Senders;
using AuthorizationServer;
using LogServer.Listeners;

namespace LogServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var localEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 13000);
            LogListener listenerobj = new LogListener(4999, localEp);
            listenerobj.ListenUdpAsync();
            Console.ReadLine();
        }
    }
}
