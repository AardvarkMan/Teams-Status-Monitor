using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StatusMonitor
{
    public class UDPInterface
    {
        Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        public UDPInterface()
        {

        }

        public void SendPacket(string Address, int Port, string Message)
        {
            IPAddress serverAddr = IPAddress.Parse(Address);

            IPEndPoint endPoint = new IPEndPoint(serverAddr, Port);

            string text = Message;
            byte[] send_buffer = Encoding.ASCII.GetBytes(text);

            sock.SendTo(send_buffer, endPoint);
        }
    }
}
