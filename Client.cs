using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Messanger
{
    public static class Client
    {
        private static bool _isRunning = true;

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

                    var messageList = new List<char>();
                    while (true)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        char keyChar = key.KeyChar;

                        if (keyChar != ' ')
                        {
                            messageList.Add(keyChar);
                            Console.Write(keyChar);
                        }

                        if (key.Key == ConsoleKey.Spacebar)
                        {
                            messageList.Add(' ');
                            Console.Write(" ");
                            //Console.WriteLine("spacebar");
                        }
                        else
                        if (key.Key == ConsoleKey.Backspace && messageList.Count > 0)
                        {
                            messageList.RemoveAt(messageList.Count - 1);
                            Console.Write("\b \b");
                            //Console.WriteLine("back");
                        }
                        else
                        if (key.Key == ConsoleKey.Enter)
                        {
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.Write("\r" + new string(' ', Console.BufferWidth) + "\r");
                            break;
                        }
                    }

                    string message = new string(messageList.ToArray());

                    if (string.IsNullOrWhiteSpace(message) || message.Replace(" ", "") == null)
                    {
                        Console.Write(new string(' ', Console.WindowWidth));
                        Console.SetCursorPosition(0, Console.CursorTop);
                        continue;
                    }

                    //Console.SetCursorPosition(0, Console.CursorTop);
                    //Console.Write(new string(' ', Console.BufferWidth));
                    //Console.SetCursorPosition(0, Console.CursorTop);

                    await writer.WriteLineAsync(Protocol.BuildProtocolString(ServerCommands.SendMessage, clientName, message));

                    //await writer.WriteLineAsync(message);

                    //if (message == "/q")
                    //{
                    //    _isRunning = false;
                    //    break;
                    //}
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

                SaveEnteredText(time, response);
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
