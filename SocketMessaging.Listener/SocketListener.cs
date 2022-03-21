using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketMessaging.Listener
{
    public class SocketListener : IDisposable
    {
        private const int BUFFERSIZE = 1024;
        private Socket listener;

        public delegate void ConnectionAcceptedHandler(string remoteIP);
        public event ConnectionAcceptedHandler ConnectionAccepted;

        public delegate void MessageReceivedHandler(object sender, MessageReceivedEventArgs e);
        public event MessageReceivedHandler MessageReceived;

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

            StartReceiveing(e.AcceptSocket);
        }

        private void StartReceiveing(Socket socket)
        {
            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.Completed += ReceiveCallback;
            e.SetBuffer(new byte[BUFFERSIZE], 0, BUFFERSIZE);
            if (!socket.ReceiveAsync(e))
            {
                ReceiveCallback(socket, e);
            }
        }

        private void ReceiveCallback(object sender, SocketAsyncEventArgs e)
        {
            Socket socket = (Socket)sender;

            try
            {
                int charsReceived = ReceiveMessage(e);

                // TODO: send confirmation

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("SocketListener", ex.Message, EventLogEntryType.Error);
            }

            StartReceiveing(socket);
        }

        private int ReceiveMessage(SocketAsyncEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred);

            MessageReceivedHandler handler = MessageReceived;
            handler?.Invoke(this, new MessageReceivedEventArgs(message));

            return message.Length;
        }

        public void Dispose()
        {
            listener?.Close();
        }
    }
}
