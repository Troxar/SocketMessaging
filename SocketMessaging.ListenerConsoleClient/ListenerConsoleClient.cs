using SocketMessaging.Listener;
using System;

namespace SocketMessaging.ListenerConsoleClient
{
    class ListenerConsoleClient
    {
        private static int PortNumber;
        private static SocketListener listener;

        static void Main()
        {
            if (!AskForPortNumber())
            {
                Console.ReadLine();
                return;
            }
            
            listener = GetListener();

            if (listener != null)
            {
                listener.ConnectionAccepted += ConnectionAccepted;
                listener.MessageReceived += MessageReceived;
                listener.ConnectionDropped += ConnectionDropped;

                WriteLine($"Listening on {PortNumber} port\n", ConsoleColor.Green);
            }

            Console.ReadLine();

            listener.StopListening();
        }

        private static bool AskForPortNumber()
        {
            Write("Enter a port number: ", ConsoleColor.Yellow);
            string input = Console.ReadLine();

            try
            {
                PortNumber = int.Parse(input);
            }
            catch (Exception ex)
            {
                WriteLine($"\n{ex.Message}", ConsoleColor.Red);
                return false;
            }

            return true;
        }

        private static SocketListener GetListener()
        {
            SocketListener socket = new SocketListener();

            try
            {
                socket.StartListening(PortNumber);
            }
            catch (Exception ex)
            {
                WriteLine($"\n{ex.Message}", ConsoleColor.Red);
                return null;
            }

            return socket;
        }

        private static void ConnectionAccepted(string remoteIP)
        {
            Write("Connection accepted: ", ConsoleColor.Green);
            WriteLine(remoteIP);
        }

        private static void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Write("Message received: ", ConsoleColor.Blue);
            WriteLine(e.Message);
        }

        private static void ConnectionDropped()
        {
            WriteLine("\nConnection dropped by client\n", ConsoleColor.Green);
            listener.StartAccepting();
            WriteLine($"Listening on {PortNumber} port\n", ConsoleColor.Green);
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
