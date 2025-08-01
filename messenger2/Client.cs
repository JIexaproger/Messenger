using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace messanger2
{
    public class Client
    {
        public const string appClientName = "Breeze client";
        public const string appVersion = "1.0";
        public const string appVersionDate = "1.08.2025";
        public const string appAuthor = "JIexaproger";


        public string _ip { get; private set; }
        public int _port { get; private set; }
        public bool _debug { get; private set; } = false;

        // запущен ли клиент
        private bool _isRunning = true;

        public Client(string ip, int port, bool debugMode)
        {
            _ip = ip;
            _port = port;
            _debug = debugMode;
        }
        public Client() { }

        private async Task<TcpClient> ConnectCLient()
        {
            TcpClient client = new TcpClient();
            try
            {
                await client.ConnectAsync(_ip, _port);
                Console.WriteLine($"Успешно подключен к серверу! ".Color(ConsoleRenderer.Color.Green) + $"{_ip} {_port}".Color(ConsoleRenderer.Color.Orange));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка подключения: {ex}".Color(ConsoleRenderer.Color.LightRed));
            }
            return client;
        }

        public async Task StartClient()
        {
            Console.Clear();
            Console.WriteLine($"{appClientName.Color(ConsoleRenderer.Color.LightViolet)} ver {appVersion}\n{appVersionDate} by {appAuthor.Color(ConsoleRenderer.Color.LightGreen)}");

            if (_ip is null || _port == 0)
            {
                SetupConnection();
            }

            TcpClient client = await ConnectCLient();

            using (client)
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream) { AutoFlush = true })
            {
                await SetupName(reader, writer);

                _ = Task.Run(() => ServerListen(reader));
                while (true)
                {
                    var mes = new Protocol(
                    ServerCommand.SendMessage,
                    message: Console.ReadLine());

                    string json = JsonSerializer.Serialize(mes);
                    Console.WriteLine(json);

                    await writer.WriteLineAsync(json);
                }
            }




            Console.ReadLine();
        }

        private void SetupConnection()
        {
            Console.WriteLine($"\n=== Настройка подключения к серверу ===".Color(ConsoleRenderer.Color.LightCyan));
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            while (true)
            {
                Console.Write("Введите IP сервера: ");
                var ipInput = Console.ReadLine();
                if (string.IsNullOrEmpty(ipInput))
                {
                    Console.SetCursorPosition(x, y);
                    continue;
                }
                _ip = ipInput;

                Console.Write("Введите порт сервера: ");
                try
                {
                    var portInput = Convert.ToInt32(Console.ReadLine());
                    if (portInput <= 0 || portInput > 65535)
                    {
                        Console.SetCursorPosition(x, y);

                        continue;
                    }
                    _port = portInput;
                }
                catch (Exception)
                {
                    Console.SetCursorPosition(x, y);
                    continue;
                }

                break;

            }
        }

        private async Task SetupName(StreamReader reader, StreamWriter writer)
        {
            Console.Write("Введите имя: ");
            var name = Console.ReadLine();

            var mes = new Protocol(
                ServerCommand.UserLogin,
                name);
            string json = JsonSerializer.Serialize(mes);
            Console.WriteLine(json);

            await writer.WriteLineAsync(json);

            Console.WriteLine(await reader.ReadLineAsync());

        }

        private async Task ServerListen(StreamReader reader)
        {
            while (true)
            {
                var response = await reader.ReadLineAsync();

                Console.WriteLine(response.Color(ConsoleRenderer.Color.LightSeaGreen));
            }
        }
    }










    //public static async Task NewClient(string ip, int port)
    //{
    //    try
    //    {
    //        using TcpClient client = new TcpClient(); // инициализация клиента

    //        await client.ConnectAsync(ip, port); // подключение клиента к серверу
    //        Console.WriteLine("Подключен к " + client.Client.RemoteEndPoint + "\n");

    //        var stream = client.GetStream();  // получение потока, потока чтения, потока отправки
    //        using var reader = new StreamReader(stream);
    //        using var writer = new StreamWriter(stream) { AutoFlush = true };

    //        string clientName = "UnregisteredUser";

    //        bool _nameVerification = true;
    //        while (_nameVerification)
    //        {
    //            string answer;
    //            Console.Write("Введите ваше имя: ");
    //            clientName = Console.ReadLine();

    //            await writer.WriteLineAsync(Protocol.BuildProtocolString(
    //                        ServerCommands.UserLogin, clientName)); // отправка имени клиента серверу

    //            while (true)
    //            {
    //                answer = await reader.ReadLineAsync();

    //                var protocol = new Protocol(answer);

    //                if (protocol.Command == ServerCommands.VerifiedLogin)
    //                {
    //                    if (protocol.Status == false)
    //                    {
    //                        Console.WriteLine("\n" + protocol.Message);
    //                        break;
    //                    }
    //                    _nameVerification = false;
    //                    break;
    //                }
    //            }
    //        }

    //        _ = Task.Run(async () => ReceiveMessagesAsync(reader));


    //        var tColor = Console.ForegroundColor;
    //        Console.ForegroundColor = ConsoleColor.Cyan;
    //        Console.WriteLine("Вы зашли в чат. Поздоровайтесь со всеми!");

    //        Console.ForegroundColor = tColor;



    //        while (_isRunning)
    //        {
    //            Console.Write("> ");
    //            inputBuffer = "";
    //            while (true)
    //            {
    //                ConsoleKeyInfo key = Console.ReadKey(true);
    //                char keyChar = key.KeyChar;

    //                if (keyChar != ' ')
    //                {
    //                    inputBuffer += keyChar;
    //                    Console.Write(keyChar);
    //                }

    //                if (key.Key == ConsoleKey.Spacebar)
    //                {
    //                    inputBuffer += " ";
    //                    Console.Write(" ");
    //                }
    //                else
    //                if (key.Key == ConsoleKey.Backspace && inputBuffer.Length > 1)
    //                {
    //                    inputBuffer = inputBuffer.Substring(0, inputBuffer.Length - 2);
    //                    //Console.WriteLine();
    //                    //Console.WriteLine(inputBuffer);
    //                    //Console.WriteLine();
    //                    Console.SetCursorPosition(0, Console.CursorTop);
    //                    Console.Write(new string(' ', 20));
    //                    Console.SetCursorPosition(0, Console.CursorTop);

    //                    Console.Write("> " + inputBuffer);
    //                }
    //                else
    //                if (key.Key == ConsoleKey.Enter)
    //                {
    //                    Console.SetCursorPosition(0, Console.CursorTop);
    //                    Console.Write(new string(' ', 20));
    //                    Console.SetCursorPosition(0, Console.CursorTop);
    //                    break;
    //                }
    //            }

    //            string message = new string(inputBuffer.ToArray());

    //            if (string.IsNullOrWhiteSpace(message) || message.Replace(" ", "") == "")
    //            {
    //                Console.Write(new string(' ', 20));
    //                Console.SetCursorPosition(0, Console.CursorTop);
    //                continue;
    //            }

    //            await writer.WriteLineAsync(Protocol.BuildProtocolString(
    //                ServerCommands.SendMessage, clientName, message));
    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Ошибка: {ex.Message}");
    //    }

    //}




    //private static async Task ReceiveMessagesAsync(StreamReader reader)
    //{
    //    while (_isRunning)
    //    {
    //        string response = await reader.ReadLineAsync();

    //        DateTime time = DateTime.Now;

    //        int cursorX = Console.CursorLeft;
    //        int cursorY = Console.CursorTop;

    //        Console.SetCursorPosition(0, Console.CursorTop);
    //        Console.Write(new string(' ', 20));
    //        Console.SetCursorPosition(0, Console.CursorTop);

    //        var tColor = Console.ForegroundColor;

    //        Console.ForegroundColor = ConsoleColor.Green;
    //        Console.Write($"[{time.Hour.ToString("00")}:{time.Minute.ToString("00")}] ");

    //        Console.ForegroundColor = tColor;

    //        var protocol = new Protocol(response);

    //        switch (protocol.Command)
    //        {
    //            case ServerCommands.UserJoin:
    //                Console.WriteLine(protocol.Name + " присоеденился к чату");
    //                break;

    //            case ServerCommands.UserLeave:
    //                Console.WriteLine(protocol.Name + " покинул чат");
    //                break;

    //            case ServerCommands.SendMessage:
    //                Console.Write($"<{protocol.Name}>: {protocol.Message}");
    //                break;
    //        }

    //        Console.Write("> " + new string(inputBuffer.ToArray()));

    //        Console.SetCursorPosition(cursorX, cursorY + 2);
    //    }
    //}

    //private static void SaveEnteredText(DateTime time, string response, List<char> message)
    //{
    //    Console.SetCursorPosition(0, Console.CursorTop);
    //    Console.Write(new string(' ', Console.WindowWidth));

    //    var tColor = Console.ForegroundColor;

    //    Console.ForegroundColor = ConsoleColor.Green;
    //    Console.Write($"[{time.Hour.ToString("00")}:{time.Minute.ToString("00")}] ");

    //    Console.ForegroundColor = tColor;
    //    Console.WriteLine(response);

    //    Console.Write("> " + message);
    //}
}
