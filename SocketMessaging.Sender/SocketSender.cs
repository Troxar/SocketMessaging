using System.Net;
using System.Net.Sockets;
using System.Text;

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

        public void SendMessage(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            sender.Send(buffer);

            // TODO: get answer

        }

        public void Dispose()
        {
            sender?.Close();
        }
    }
}
