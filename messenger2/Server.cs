using messenger2;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace messanger2
{
    public class Server
    {
        private static object locker = new();

        private readonly IPAddress _ip;
        private readonly int _port;
        private readonly bool _isRunning;

        public int MaxNameLength = 512;
        public int MinNameLength = 1;

        private Dictionary<string, ClientConfig> clients;



        public Server(string ip, int port)
        {
            _ip = IPAddress.Parse(ip);
            _port = port;
            clients = new Dictionary<string, ClientConfig>();
        }
        public Server(IPAddress ip, int port)
        {
            _ip = ip;
            _port = port;
            clients = new Dictionary<string, ClientConfig>();
        }



        public void CreateServer()
        {
            TcpListener server = new TcpListener(_ip, _port);
            server.Start();
            Console.WriteLine($"Сервер запущен на порту {_port} ({server.LocalEndpoint})");
            _ = HandleServerConsoleAsync();
            _ = AcceptClientsAsync(server);
        }

        private async Task AcceptClientsAsync(TcpListener server)
        {
            while (_isRunning)
            {
                var client = await server.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }

        private async Task<string> ClientAuthorizationAsync(StreamReader reader, StreamWriter writer)
        {
            string userName = "UnregisteredUser";
            while (_isRunning)
            {
                string? userInput = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(userInput)) continue;

                userName = userInput;

                bool nameNotAvailable = false;
                lock (locker)
                {
                    foreach (var client in clients)
                    {
                        if (client.Key == userName)
                        {
                            nameNotAvailable = true;
                            break;
                        }
                    }
                }

                if (nameNotAvailable)
                {
                    await writer.WriteLineAsync(Protocol.BuildProtocolString(
                        ServerCommand.VerifiedLogin, status: false, error: Error.NameNotAvailable));
                    continue;
                }
                if (userName.Length > MaxNameLength)
                {
                    await writer.WriteLineAsync(Protocol.BuildProtocolString(
                        ServerCommand.VerifiedLogin, status: false, error: Error.NameTooLong));
                    continue;
                }
                if (userName.Length < MinNameLength)
                {
                    await writer.WriteLineAsync(Protocol.BuildProtocolString
                        (ServerCommand.VerifiedLogin, status: false, error: Error.NameTooLong));
                    continue;
                }
                if (string.IsNullOrEmpty(userName))
                {
                    await writer.WriteLineAsync(Protocol.BuildProtocolString(
                        ServerCommand.VerifiedLogin, status: false, error: Error.NameIsEmpty));
                    continue;
                }

                break;
            }
            return userName;
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream) { AutoFlush = true })
            {
                string clientName = await ClientAuthorizationAsync(reader, writer);

                clients.Add(clientName, new ClientConfig(client, reader, writer));

                while (_isRunning)
                {
                    string? response = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(response)) continue;

                    Protocol responseProtocol = new Protocol(response);

                    if (responseProtocol.Command == ServerCommand.SendMessage)
                    {
                        if (string.IsNullOrEmpty(responseProtocol.Message)) continue;

                        await writer.WriteLineAsync(response);
                        continue;
                    }
                    else
                    if (responseProtocol.Command == ServerCommand.SendCommand)
                    {
                        if (string.IsNullOrEmpty(responseProtocol.Message)) continue;

                        string[] args = responseProtocol.Message.Split(' ');

                        if (args[0] == "online")
                        {
                            await writer.WriteLineAsync(Protocol.BuildProtocolString());
                        }
                    }

                }

            }
        }

        private async Task HandleServerConsoleAsync()
        {
            while (_isRunning)
            {
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;

                string[] inputParts = input.Split(' ');

                if (inputParts[0] == "send")
                {
                    Console.WriteLine(inputParts[1]);
                }
            }

        }

        private async Task BoardcastAsync(Protocol protocol)
        {
            foreach (var client in clients)
            {
                await client.Value.GetStreamWriter().WriteLineAsync(Protocol.BuildProtocolString());
            }
        }


        //public static async Task CreateServer1(string ip = "0.0.0.0", int port = 5959)
        //{
        //    TcpListener server = new TcpListener(IPAddress.Parse(ip), port);
        //    server.Start();
        //    Console.WriteLine($"Сервер запущен на порту {port} ({server.LocalEndpoint})");

        //    try
        //    {
        //        _ = AcceptClientsAsync1(server);

        //        string consoleInput;
        //        while (true)
        //        {
        //            consoleInput = Console.ReadLine();

        //            if (consoleInput == "Close")
        //            {
        //                break;
        //            }
        //            else await BroadcastAsync(ServerCommand.SendMessage, "CONSOLE", consoleInput);
        //        }
        //    }
        //    finally
        //    {
        //        server.Stop();
        //    }
        //}

        //private static async Task AcceptClientsAsync1(TcpListener server)
        //{
        //    while (true)
        //    {
        //        var client = await server.AcceptTcpClientAsync();
        //        _ = HandleClientAsync(client); // Не ждём завершения задачи    
        //    }
        //}

        //private static async Task HandleClientAsync(TcpClient client)
        //{
        //    using (client)
        //    using (var stream = client.GetStream())
        //    using (var reader = new StreamReader(stream))
        //    using (var writer = new StreamWriter(stream) { AutoFlush = true })
        //    {
        //        //bool emptyName = false;
        //        string clientName;
        //        while (true)
        //        {
        //            while (true)
        //            {
        //                clientName = await reader.ReadLineAsync();

        //                var protocol = new Protocol(clientName);

        //                if (protocol.Command == ServerCommand.UserLogin)
        //                    clientName = protocol.Name;
        //                    break;
        //            }


        //            if (clientName == "исключение")
        //            {
        //                Console.WriteLine("isklu");
        //                if (maxNameLenght >= 96)
        //                {
        //                    await writer.WriteLineAsync(Protocol.BuildProtocolString(
        //                        ServerCommand.VerifiedLogin, false, message: "Не наглей..."));
        //                    continue;
        //                }
        //                else await writer.WriteLineAsync(Protocol.BuildProtocolString(
        //                       ServerCommand.VerifiedLogin, false, message: "Исключение принято"));
        //                maxNameLenght += 8;
        //                minNameLength = 0;
        //                emptyName = false;
        //                continue;
        //            }
        //            else if (clientName.Replace(" ", "").Length >= maxNameLenght || clientName.Length >= maxNameLenght)
        //            {
        //                await writer.WriteLineAsync(Protocol.BuildProtocolString(
        //                        ServerCommand.VerifiedLogin, false, message: $"Ваше имя не должно превышать {maxNameLenght} символов!"));
        //                continue;
        //            }
        //            else if (clientName.Replace(" ", "").Length < minNameLength)
        //            {
        //                await writer.WriteLineAsync(Protocol.BuildProtocolString(
        //                        ServerCommand.VerifiedLogin, false, message: $"В вашем имени должно быть больше {minNameLength - 1} символов!"));
        //                continue;
        //            }
        //            else if (clientName.Replace(" ", "").Length < clientName.Length / 2 && emptyName)
        //            {
        //                await writer.WriteLineAsync(Protocol.BuildProtocolString(
        //                        ServerCommand.VerifiedLogin, false, message: "Ваше имя не должно быть пустым!"));
        //                continue;
        //            }
        //            else
        //            {
        //                Console.WriteLine("samoe to!");
        //                clients.TryAdd(client, writer);
        //                await writer.WriteLineAsync(Protocol.BuildProtocolString(
        //                        ServerCommand.VerifiedLogin, true));
        //                break;
        //            }
        //        }


        //        var tColor = Console.ForegroundColor;

        //        DateTime now = DateTime.Now;

        //        Console.ForegroundColor = ConsoleColor.Green;
        //        Console.Write($"[{now.Hour.ToString("00")}:{now.Minute.ToString("00")}:{now.Second.ToString("00")}] ");
        //        Console.ForegroundColor = ConsoleColor.Cyan;
        //        Console.WriteLine($"{clientName} подключился");

        //        Console.ForegroundColor = tColor;

        //        await BroadcastAsync(ServerCommand.UserJoin, clientName, null); // сообщение всем

        //        try
        //        {
        //            while (client.Connected)
        //            {
        //                var answer = await reader.ReadLineAsync();
        //                var protocol = new Protocol(answer);
        //                Console.WriteLine(answer);
        //                if (string.IsNullOrEmpty(protocol.Message)) continue;


        //                DateTime time = DateTime.Now;

        //                //if (message == "/q")
        //                //{
        //                //    tColor = Console.ForegroundColor;

        //                //    Console.ForegroundColor = ConsoleColor.Green;
        //                //    Console.Write($"[{time.Hour.ToString("00")}:{time.Minute.ToString("00")}:{time.Second.ToString("00")}] ");

        //                //    Console.ForegroundColor = ConsoleColor.Red;
        //                //    Console.Write("[ COMMAND ] ");

        //                //    Console.ForegroundColor = ConsoleColor.DarkYellow;
        //                //    Console.WriteLine($"{clientName} : {message}");

        //                //    Console.ForegroundColor = tColor;
        //                //    break;
        //                //}

        //                Console.ForegroundColor = ConsoleColor.Green;
        //                Console.Write($"[{time.Hour.ToString("00")}:{time.Minute.ToString("00")}:{time.Second.ToString("00")}] ");

        //                Console.ForegroundColor = tColor;
        //                Console.Write($"<{clientName}>: {protocol.Message}\n");

        //                await BroadcastAsync(ServerCommand.SendMessage, clientName, protocol.Message);
        //            }
        //        }
        //        finally
        //        {
        //            clients.TryRemove(client, out _);
        //            Console.ForegroundColor = ConsoleColor.Green;
        //            Console.Write($"[{now.Hour.ToString("00")}:{now.Minute.ToString("00")}:{now.Second.ToString("00")}] ");
        //            Console.ForegroundColor = ConsoleColor.Red;
        //            Console.WriteLine($"{clientName} отключился");

        //            Console.ForegroundColor = tColor;

        //            await BroadcastAsync(ServerCommand.UserLeave, clientName, null); // сообщение всем
        //        }
        //    }
        //}

        //private static async Task BroadcastAsync(ServerCommand serverCommand, string name, string message)
        //{
        //    var sendProtocol = Protocol.BuildProtocolString(serverCommand, name, message);

        //    foreach (var client in clients)
        //    {
        //        //if (client.Key != sender && client.Key.Connected)
        //        if (client.Key.Connected)
        //        {
        //            try
        //            {
        //                await client.Value.WriteLineAsync(sendProtocol);
        //            }
        //            catch { /* Игнорируем ошибки отправки */ }
        //        }
        //    }
        //}
    }
}
