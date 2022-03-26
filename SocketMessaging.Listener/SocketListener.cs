using System;
using System.Diagnostics;
using System.Linq;
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

        public delegate void ConnectionDroppedHandler();
        public event ConnectionDroppedHandler ConnectionDropped;

        private IPAddress GetHostIPAddress()
        {
            IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
            return entry.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
        }

        public void StartListening(int port)
        {
            IPAddress address = GetHostIPAddress();
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

            if (!IsSocketConnected(socket))
            {
                ConnectionDroppedHandler handler = ConnectionDropped;
                handler?.Invoke();

                return;
            }

            try
            {
                int charsReceived = ReceiveMessage(e);
                SendConfirmation(socket, charsReceived);
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

        private void SendConfirmation(Socket socket, int charsReceived)
        {
            string reply = $"Received {charsReceived} character(s)";
            byte[] buffer = Encoding.UTF8.GetBytes(reply);
            socket.Send(buffer);
        }

        private bool IsSocketConnected(Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException)
            {
                return false;
            }
        }

        public void StopListening()
        {
            listener?.Close();
        }

        public void Dispose()
        {
            StopListening();
        }
    }
}
