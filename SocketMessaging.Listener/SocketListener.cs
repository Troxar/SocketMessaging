using System;
using System.Net;
using System.Net.Sockets;

namespace SocketMessaging.Listener
{
    public class SocketListener : IDisposable
    {
        private Socket listener;

        public delegate void ConnectionAcceptedHandler(string remoteIP);
        public event ConnectionAcceptedHandler ConnectionAccepted;

        public void StartListening(string hostname, int port)
        {
            IPHostEntry entry = Dns.GetHostEntry(hostname);
            IPAddress address = entry.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(address, port);

            listener = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(endPoint);
            listener.Listen(10);

            StartAccepting();
        }

        public void StartAccepting()
        {
            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.Completed += AcceptCallback;
            if (!listener.AcceptAsync(e))
            {
                AcceptCallback(listener, e);
            }
        }

        private void AcceptCallback(object sender, SocketAsyncEventArgs e)
        {
            ConnectionAcceptedHandler handler = ConnectionAccepted;
            if (handler != null)
            {
                string remoteIP = ((IPEndPoint)e.AcceptSocket.RemoteEndPoint).Address.ToString();
                handler.Invoke(remoteIP);
            }

            // TODO: start receiving
        }

        public void Dispose()
        {
            listener?.Close();
        }
    }
}
