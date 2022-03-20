using System.Net;
using System.Net.Sockets;

namespace SocketMessaging.Sender
{
    public class SocketSender
    {
        private Socket sender;

        public void Initialize(string hostname, int port)
        {
            IPHostEntry entry = Dns.GetHostEntry(hostname);
            IPAddress address = entry.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(address, port);

            sender = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(endPoint);
        }

        public void Dispose()
        {
            sender?.Close();
        }
    }
}
