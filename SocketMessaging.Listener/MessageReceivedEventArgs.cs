using System;

namespace SocketMessaging.Listener
{
    public class MessageReceivedEventArgs : EventArgs
    {
        private readonly string message;

        public MessageReceivedEventArgs(string message)
        {
            this.message = message;
        }

        public string Message
        {
            get { return message; }
        }
    }
}
