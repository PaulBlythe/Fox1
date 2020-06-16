using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TestHost
{
    public class UDPComms
    {
        public enum MessageType
        {
            Connect,
            DisConnect,
            Subscribe,
            Unknown
        }

        UdpClient sender;
        
        List<String> ConnectedDevices = new List<string>();
        public List<ConnectedDeviceStreamHandler> Handlers = new List<ConnectedDeviceStreamHandler>();

        public UDPComms()
        {
            sender = new UdpClient();
        }

        public void BroadcastMessage()
        {
            foreach(ConnectedDeviceStreamHandler handler in Handlers)
            {
                handler.Update(sender);
            }
        }

        public String HandleMessage(String ip, String msg, out MessageType type)
        {
            String result ="";
            type = MessageType.Unknown;

            if (ConnectedDevices.Contains(ip))
            {
                string[] parts = msg.Split(':');
                switch (parts[0])
                {
                    case "want":
                        {
                            int i = ConnectedDevices.IndexOf(ip);
                            type = MessageType.Subscribe;
                            result = parts[1];
                            Handlers[i].AddStream(parts[1]);
                        }
                        break;
                }
            }
            else
            {
                type = MessageType.Connect;
                result = ip;
                ConnectedDevices.Add(ip);
                ConnectedDeviceStreamHandler handler = new ConnectedDeviceStreamHandler(ip);
                Handlers.Add(handler);
            }
            return result;
        }
    }
}
