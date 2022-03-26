using SocketMessaging.Sender;
using System;

namespace SocketMessaging.SenderConsoleClient
{
    class SenderConsoleClient
    {
        private static string HostName;
        private static int PortNumber;

        private static SocketSender sender;

        static void Main()
        {
            if (!AskForConnectionParams())
            {
                Console.ReadLine();
                return;
            }

            sender = GetSender();

            if (sender == null)
            {
                Console.ReadLine();
                return;
            }

            StartCommunicating();
        }

        private static bool AskForConnectionParams()
        {
            Write("Enter a host name or IP address: ", ConsoleColor.Yellow);
            HostName = Console.ReadLine();

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

        private static SocketSender GetSender()
        {
            SocketSender socket = new SocketSender();

            try
            {
                socket.Initialize(HostName, PortNumber);
            }
            catch (Exception ex)
            {
                WriteLine($"\n{ex.Message}", ConsoleColor.Red);
                return null;
            }

            return socket;
        }

        static void StartCommunicating()
        {
            WriteLine($"Ready to communicate - {HostName}:{PortNumber}", ConsoleColor.Green);

            while (sender != null)
            {
                Write("\nEnter a message ('q' for quit): ", ConsoleColor.Yellow);
                string message = Console.ReadLine();

                if (string.IsNullOrEmpty(message))
                {
                    continue;
                }
                else if (message == "q")
                {
                    sender.CloseConnection();
                    break;
                }

                GetAnswerResult result = sender.SendMessageAndGetAnswer(message);

                if (result.IsSuccessful)
                {
                    Write("Answer received: ", ConsoleColor.Blue);
                    WriteLine(result.Answer);
                }
                else
                {
                    WriteLine(result.Answer, ConsoleColor.Red);
                }
            }
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
