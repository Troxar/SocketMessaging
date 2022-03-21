﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketMessaging.Sender
{
    public class SocketSender
    {
        private const int BUFFERSIZE = 1024;
        private Socket sender;

        public void Initialize(string hostname, int port)
        {
            IPHostEntry entry = Dns.GetHostEntry(hostname);
            IPAddress address = entry.AddressList[0];
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

        public void Dispose()
        {
            sender?.Close();
        }
    }
}
