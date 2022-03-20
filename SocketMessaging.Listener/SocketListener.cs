using System;
using System.Net;
using System.Net.Sockets;

namespace SocketMessaging.Listener
{
    public class SocketListener : IDisposable
    {
        private Socket listener;

        public void StartListening(string hostname, int port)
        {
            IPHostEntry entry = Dns.GetHostEntry(hostname);
            IPAddress address = entry.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(address, port);

            listener = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(endPoint);
            listener.Listen(10);
        }

        public void Dispose()
        {
            listener?.Close();
        }
    }
}
