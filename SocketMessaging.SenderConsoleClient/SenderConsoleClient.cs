using SocketMessaging.Sender;
using System;

namespace SocketMessaging.SenderConsoleClient
{
    class SenderConsoleClient
    {
        private const string HOSTNAME = "localhost";
        private const int PORTNUMBER = 11000;

        private static SocketSender sender;

        static void Main()
        {
            sender = GetSender();

            if (sender != null)
            {
                WriteLine($"Ready to communicate - {HOSTNAME}:{PORTNUMBER}", ConsoleColor.Green);
            }

            Console.ReadLine();

            sender?.Dispose();
        }

        private static SocketSender GetSender()
        {
            SocketSender socket = new SocketSender();

            try
            {
                socket.Initialize(HOSTNAME, PORTNUMBER);
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message, ConsoleColor.Red);
                return null;
            }

            return socket;
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
