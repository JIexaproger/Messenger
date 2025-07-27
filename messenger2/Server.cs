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

namespace messanger1
{
    public static class Server
    {
        private static ConcurrentDictionary<TcpClient, StreamWriter> clients
            = new ConcurrentDictionary<TcpClient, StreamWriter>();

        public static async Task CreateServer(string ip = "0.0.0.0", int port = 5959)
        {
            TcpListener server = new TcpListener(IPAddress.Parse(ip), port);
            server.Start();
            Console.WriteLine($"Сервер запущен на порту {port} ({server.LocalEndpoint})");

            try
            {
                _ = AcceptClientsAsync(server);

                string consoleInput;
                while (true)
                {
                    consoleInput = Console.ReadLine();

                    if (consoleInput == "Close")
                    {
                        break;
                    }
                    else await BroadcastAsync(ServerCommands.SendMessage, "CONSOLE", consoleInput);
                }
            }
            finally
            {
                server.Stop();
            }
        }

        private static async Task AcceptClientsAsync(TcpListener server)
        {
            while (true)
            {
                var client = await server.AcceptTcpClientAsync();
                _ = HandleClientAsync(client); // Не ждём завершения задачи    
            }
        }

        private static async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream) { AutoFlush = true })
            {
                bool emptyName = false;
                int maxNameLenght = 255;
                int minNameLength = 1;
                string clientName;
                while (true)
                {
                    while (true)
                    {
                        clientName = await reader.ReadLineAsync();

                        var protocol = new Protocol(clientName);

                        if (protocol.Command == ServerCommands.UserLogin)
                            clientName = protocol.Name;
                            break;
                    }
                    

                    if (clientName == "исключение")
                    {
                        Console.WriteLine("isklu");
                        if (maxNameLenght >= 96)
                        {
                            await writer.WriteLineAsync(Protocol.BuildProtocolString(
                                ServerCommands.VerifiedLogin, false, message: "Не наглей..."));
                            continue;
                        }
                        else await writer.WriteLineAsync(Protocol.BuildProtocolString(
                               ServerCommands.VerifiedLogin, false, message: "Исключение принято"));
                        maxNameLenght += 8;
                        minNameLength = 0;
                        emptyName = false;
                        continue;
                    }
                    else if (clientName.Replace(" ", "").Length >= maxNameLenght || clientName.Length >= maxNameLenght)
                    {
                        await writer.WriteLineAsync(Protocol.BuildProtocolString(
                                ServerCommands.VerifiedLogin, false, message: $"Ваше имя не должно превышать {maxNameLenght} символов!"));
                        continue;
                    }
                    else if (clientName.Replace(" ", "").Length < minNameLength)
                    {
                        await writer.WriteLineAsync(Protocol.BuildProtocolString(
                                ServerCommands.VerifiedLogin, false, message: $"В вашем имени должно быть больше {minNameLength - 1} символов!"));
                        continue;
                    }
                    else if (clientName.Replace(" ", "").Length < clientName.Length / 2 && emptyName)
                    {
                        await writer.WriteLineAsync(Protocol.BuildProtocolString(
                                ServerCommands.VerifiedLogin, false, message: "Ваше имя не должно быть пустым!"));
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("samoe to!");
                        clients.TryAdd(client, writer);
                        await writer.WriteLineAsync(Protocol.BuildProtocolString(
                                ServerCommands.VerifiedLogin, true));
                        break;
                    }
                }


                var tColor = Console.ForegroundColor;

                DateTime now = DateTime.Now;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"[{now.Hour.ToString("00")}:{now.Minute.ToString("00")}:{now.Second.ToString("00")}] ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{clientName} подключился");

                Console.ForegroundColor = tColor;

                await BroadcastAsync(ServerCommands.UserJoin, clientName, null); // сообщение всем

                try
                {
                    while (client.Connected)
                    {
                        var answer = await reader.ReadLineAsync();
                        var protocol = new Protocol(answer);
                        Console.WriteLine(answer);
                        if (string.IsNullOrEmpty(protocol.Message)) continue;


                        DateTime time = DateTime.Now;

                        //if (message == "/q")
                        //{
                        //    tColor = Console.ForegroundColor;

                        //    Console.ForegroundColor = ConsoleColor.Green;
                        //    Console.Write($"[{time.Hour.ToString("00")}:{time.Minute.ToString("00")}:{time.Second.ToString("00")}] ");

                        //    Console.ForegroundColor = ConsoleColor.Red;
                        //    Console.Write("[ COMMAND ] ");

                        //    Console.ForegroundColor = ConsoleColor.DarkYellow;
                        //    Console.WriteLine($"{clientName} : {message}");

                        //    Console.ForegroundColor = tColor;
                        //    break;
                        //}

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"[{time.Hour.ToString("00")}:{time.Minute.ToString("00")}:{time.Second.ToString("00")}] ");

                        Console.ForegroundColor = tColor;
                        Console.Write($"<{clientName}>: {protocol.Message}\n");

                        await BroadcastAsync(ServerCommands.SendMessage, clientName, protocol.Message);
                    }
                }
                finally
                {
                    clients.TryRemove(client, out _);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"[{now.Hour.ToString("00")}:{now.Minute.ToString("00")}:{now.Second.ToString("00")}] ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{clientName} отключился");

                    Console.ForegroundColor = tColor;

                    await BroadcastAsync(ServerCommands.UserLeave, clientName, null); // сообщение всем
                }
            }
        }

        private static async Task BroadcastAsync(ServerCommands serverCommand, string name, string message)
        {
            var sendProtocol = Protocol.BuildProtocolString(serverCommand, name, message);

            foreach (var client in clients)
            {
                //if (client.Key != sender && client.Key.Connected)
                if (client.Key.Connected)
                {
                    try
                    {
                        await client.Value.WriteLineAsync(sendProtocol);
                    }
                    catch { /* Игнорируем ошибки отправки */ }
                }
            }
        }
    }
}
