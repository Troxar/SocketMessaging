using SocketMessaging.Listener;
using System;

namespace SocketMessaging.ListenerConsoleClient
{
    class ListenerConsoleClient
    {
        private const string HOSTNAME = "localhost";
        private const int PORTNUMBER = 11000;

        private static SocketListener listener;

        static void Main()
        {
            listener = GetListener();

            if (listener != null)
            {
                listener.ConnectionAccepted += ConnectionAccepted;
                WriteLine($"Listening on {PORTNUMBER} port\n", ConsoleColor.Green);
            }

            Console.ReadLine();

            listener?.Dispose();
        }

        private static SocketListener GetListener()
        {
            SocketListener socket = new SocketListener();

            try
            {
                socket.StartListening(HOSTNAME, PORTNUMBER);
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message, ConsoleColor.Red);
                return null;
            }

            return socket;
        }

        private static void ConnectionAccepted(string remoteIP)
        {
            Write("Connection accepted: ", ConsoleColor.Green);
            WriteLine(remoteIP);
        }

        private static void WriteLine(string value, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(value);
        }

        private static void Write(string value, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write(value);
        }
    }
}
