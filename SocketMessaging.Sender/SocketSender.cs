using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketMessaging.Sender
{
    public class SocketSender
    {
        private const int BUFFERSIZE = 1024;
        private Socket sender;

        private IPAddress GetHostIPAddress(string hostname)
        {
            IPHostEntry entry = Dns.GetHostEntry(hostname);
            return entry.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
        }

        public void Initialize(string hostname, int port)
        {
            IPAddress address = GetHostIPAddress(hostname);
            IPEndPoint endPoint = new IPEndPoint(address, port);

            sender = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(endPoint);
        }

        public GetAnswerResult SendMessageAndGetAnswer(string message)
        {
            string answer;

            try
            {
                SendMessage(message);
                answer = GetAnswer();
            }
            catch (Exception ex)
            {
                return new GetAnswerResult { IsSuccessful = false, Answer = ex.Message };
            }

            return new GetAnswerResult { IsSuccessful = true, Answer = answer };
        }

        private void SendMessage(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            sender.Send(buffer);
        }

        private string GetAnswer()
        {
            byte[] buffer = new byte[BUFFERSIZE];
            int bytesReceived = sender.Receive(buffer);

            return Encoding.UTF8.GetString(buffer, 0, bytesReceived);
        }

        public void CloseConnection()
        {
            if (sender == null)
            {
                return;
            }

            try
            {
                sender.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                sender.Close();
            }
        }

        public void Dispose()
        {
            CloseConnection();
        }
    }
}
