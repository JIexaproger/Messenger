using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
//using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace messanger1
{
    public static class Client
    {
        private static bool _isRunning = true;
        private static string inputBuffer;

        public static async Task NewClient(string ip, int port)
        {
            try
            {
                using TcpClient client = new TcpClient(); // инициализация клиента

                await client.ConnectAsync(ip, port); // подключение клиента к серверу
                Console.WriteLine("Подключен к " + client.Client.RemoteEndPoint + "\n");

                var stream = client.GetStream();  // получение потока, потока чтения, потока отправки
                using var reader = new StreamReader(stream);
                using var writer = new StreamWriter(stream) { AutoFlush = true };

                string clientName = "UnregisteredUser";

                bool _nameVerification = true;
                while (_nameVerification)
                {
                    string answer;
                    Console.Write("Введите ваше имя: ");
                    clientName = Console.ReadLine();

                    await writer.WriteLineAsync(Protocol.BuildProtocolString(
                                ServerCommands.UserLogin, clientName)); // отправка имени клиента серверу

                    while (true)
                    {
                        answer = await reader.ReadLineAsync();

                        var protocol = new Protocol(answer);

                        if (protocol.Command == ServerCommands.VerifiedLogin)
                        {
                            if (protocol.Status == false)
                            {
                                Console.WriteLine("\n" + protocol.Message);
                                break;
                            }
                            _nameVerification = false;
                            break;
                        }
                    }
                }

                _ = Task.Run(async () => ReceiveMessagesAsync(reader));


                var tColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Вы зашли в чат. Поздоровайтесь со всеми!");

                Console.ForegroundColor = tColor;



                while (_isRunning)
                {
                    Console.Write("> ");
                    inputBuffer = "";
                    while (true)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        char keyChar = key.KeyChar;

                        if (keyChar != ' ')
                        {
                            inputBuffer += keyChar;
                            Console.Write(keyChar);
                        }

                        if (key.Key == ConsoleKey.Spacebar)
                        {
                            inputBuffer += " ";
                            Console.Write(" ");
                        }
                        else
                        if (key.Key == ConsoleKey.Backspace && inputBuffer.Length > 1)
                        {
                            inputBuffer = inputBuffer.Substring(0, inputBuffer.Length - 2);
                            //Console.WriteLine();
                            //Console.WriteLine(inputBuffer);
                            //Console.WriteLine();
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.Write(new string(' ', 20));
                            Console.SetCursorPosition(0, Console.CursorTop);

                            Console.Write("> " + inputBuffer);
                        }
                        else
                        if (key.Key == ConsoleKey.Enter)
                        {
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.Write(new string(' ', 20));
                            Console.SetCursorPosition(0, Console.CursorTop);
                            break;
                        }
                    }

                    string message = new string(inputBuffer.ToArray());

                    if (string.IsNullOrWhiteSpace(message) || message.Replace(" ", "") == "")
                    {
                        Console.Write(new string(' ', 20));
                        Console.SetCursorPosition(0, Console.CursorTop);
                        continue;
                    }

                    await writer.WriteLineAsync(Protocol.BuildProtocolString(
                        ServerCommands.SendMessage, clientName, message));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

        }




        private static async Task ReceiveMessagesAsync(StreamReader reader)
        {
            while (_isRunning)
            {
                string response = await reader.ReadLineAsync();

                DateTime time = DateTime.Now;

                int cursorX = Console.CursorLeft;
                int cursorY = Console.CursorTop;

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', 20));
                Console.SetCursorPosition(0, Console.CursorTop);

                var tColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"[{time.Hour.ToString("00")}:{time.Minute.ToString("00")}] ");

                Console.ForegroundColor = tColor;

                var protocol = new Protocol(response);

                switch (protocol.Command)
                {
                    case ServerCommands.UserJoin:
                        Console.WriteLine(protocol.Name + " присоеденился к чату");
                        break;

                    case ServerCommands.UserLeave:
                        Console.WriteLine(protocol.Name + " покинул чат");
                        break;

                    case ServerCommands.SendMessage:
                        Console.Write($"<{protocol.Name}>: {protocol.Message}");
                        break;
                }

                Console.Write("> " + new string(inputBuffer.ToArray()));

                Console.SetCursorPosition(cursorX, cursorY + 2);
            }
        }

        private static void SaveEnteredText(DateTime time, string response, List<char> message)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));

            var tColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[{time.Hour.ToString("00")}:{time.Minute.ToString("00")}] ");

            Console.ForegroundColor = tColor;
            Console.WriteLine(response);

            Console.Write("> " + message);
        }
    }
}
