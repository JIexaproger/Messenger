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
using System.Text.Json;
using System.Threading.Tasks;

namespace messanger2
{
    public class Server
    {
        // статичный локер для исключения гонки за резервирование имени
        private static object registrationLocker = new();
        private static object consoleOutputLocker = new();
        private static object authorizationLocker = new();


        private readonly IPAddress _ip;
        private readonly int _port;

        // запущен ли сервер
        private bool _isRunning = true;

        // ограничения на длинну имён
        public int MaxNameLength = 512;
        public int MinNameLength = 1;

        // словарь имени клиентов/конфиг клиента
        private Dictionary<string, ClientConfig> clients;


        private int id;


        private bool _debug;

        // конструкторы
        public Server(string ip, int port, bool debugMode)
        {
            _ip = IPAddress.Parse(ip);
            _port = port;
            _debug = debugMode;
            clients = new Dictionary<string, ClientConfig>();
        }
        public Server(IPAddress ip, int port)
        {
            _ip = ip;
            _port = port;
            clients = new Dictionary<string, ClientConfig>();
        }


        // метод запуска сервера
        public async Task CreateServer()
        {
            TcpListener server = new TcpListener(_ip, _port);
            server.Start();
            Console.WriteLine($"Сервер запущен на порту {_port} ({server.LocalEndpoint})");
            _ = HandleServerConsoleAsync();
            await AcceptClientsAsync(server);
        }


        // асинхронный метод запуска приёма клиентов
        private async Task AcceptClientsAsync(TcpListener server)
        {
            while (_isRunning)
            {
                var client = await server.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }


        // асинхронный метод авторизации клиента
        private async Task<string> ClientAuthorizationAsync(TcpClient tcpClient, StreamReader reader, StreamWriter writer)
        {
            // инициализация переменной имени и стандартное значение
            Protocol? protocol;
            while (true)
            {
                // чтение сообщения клиента
                string? userInput = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(userInput)) continue;

                lock (consoleOutputLocker) { Console.WriteLine(userInput); }

                protocol = JsonSerializer.Deserialize<Protocol>(userInput);
                if (protocol is null) continue;
                if (string.IsNullOrEmpty(protocol.Name)) continue;

                if (protocol.Command != ServerCommand.UserLogin) continue;

                bool nameNotAvailable = false;

                // исключение гонки за одно имя
                lock (registrationLocker)
                {
                    foreach (var client in clients)
                    {
                        if (client.Key == protocol.Name)
                        {
                            nameNotAvailable = true;
                            break;
                        }
                    }
                }

                // обработка ошибки: имя уже занято
                if (nameNotAvailable)
                {
                    await writer.WriteLineAsync(JsonSerializer.Serialize(new Protocol(
                        ServerCommand.VerifiedLogin,
                        status: false,
                        error: Error.NameNotAvailable)));
                    continue;
                }
                // обработка ошибки: имя слишком длинное
                if (protocol.Name.Length > MaxNameLength)
                {
                    await writer.WriteLineAsync(JsonSerializer.Serialize(new Protocol(
                        ServerCommand.VerifiedLogin,
                        status: false,
                        error: Error.NameTooLong)));
                    continue;
                }
                // обработка ошибки: имя слишком короткое
                if (protocol.Name.Length < MinNameLength)
                {
                    await writer.WriteLineAsync(JsonSerializer.Serialize(new Protocol(
                        ServerCommand.VerifiedLogin,
                        status: false,
                        error: Error.NameTooShort)));
                    continue;
                }
                // обработка ошибки: пустое имя
                if (string.IsNullOrEmpty(protocol.Name))
                {
                    await writer.WriteLineAsync(JsonSerializer.Serialize(new Protocol(
                        ServerCommand.VerifiedLogin,
                        status: false,
                        error: Error.NameIsEmpty)));
                    continue;
                }

                break;
            }

            lock (authorizationLocker)
            {
                // добовление имени в словарь
                clients.Add(protocol.Name, new ClientConfig(tcpClient, reader, writer, id));
                id++;
            }

            return protocol.Name;
        }


        // асинхронный метод обработки клиента
        private async Task HandleClientAsync(TcpClient client)
        {
            // получение потоков клиента с конструкцией using, которая освободит память после выполнения блока кода 
            using (client)
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream) { AutoFlush = true })
            {
                // получение имени клиента через метод авторизации
                string clientName = await ClientAuthorizationAsync(client, reader, writer);


                await UserConnect(clientName);

                while (_isRunning)
                {
                    // чтение сообщения клиента
                    string? response = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(response)) continue;

                    lock (consoleOutputLocker) { if (_debug) Console.WriteLine(response); }

                    try
                    {
                        // расшифорвка json сообщения клиента
                        Protocol? responseProtocol = JsonSerializer.Deserialize<Protocol>(response);
                        if (responseProtocol is null) continue;


                        // если клиент отправил команду об отправке сообщения
                        if (responseProtocol.Command == ServerCommand.SendMessage)
                        {
                            if (string.IsNullOrEmpty(responseProtocol.Message)) continue;

                            var mes = JsonSerializer.Serialize(new Protocol(
                                ServerCommand.SendMessage,
                                name: clientName,
                                message: responseProtocol.Message));

                            await BoardcastAsync(mes);

                            lock (consoleOutputLocker) { Console.WriteLine(mes); }

                            continue;
                        }


                        // если клиент отправил команду об отправке команды
                        if (responseProtocol.Command == ServerCommand.SendCommand)
                        {
                            if (string.IsNullOrEmpty(responseProtocol.Message)) continue;

                            string[] args = responseProtocol.Message.Split(' ');

                            if (args[0] == "online")
                            {
                                var mes = JsonSerializer.Serialize(new Protocol(
                                    ServerCommand.ServerOnline,
                                    strings: clients.Keys.ToArray()));

                                await writer.WriteLineAsync(mes);

                                lock (consoleOutputLocker) { Console.WriteLine(mes); }

                                continue;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (_debug) { Console.WriteLine($"Неправильный JSON формат от \"{clientName}\": {response}\n{ex}"); }
                    }

                    await UserDisconnect(clientName);

                }

            }
        }

        private async Task UserConnect(string name)
        {
            var mes = JsonSerializer.Serialize(new Protocol(
                                ServerCommand.UserJoin,
                                name: name,
                                id: clients[name].GetId()));

            await BoardcastAsync(mes);
            lock (consoleOutputLocker) { Console.WriteLine(mes); }
        }
        private async Task UserDisconnect(string name)
        {
            var mes = JsonSerializer.Serialize(new Protocol(
                                ServerCommand.UserLeave,
                                name: name));

            await BoardcastAsync(mes);
            lock (consoleOutputLocker) { Console.WriteLine(mes); }
        }

        // обработчик ввода в консоль сервера
        private async Task HandleServerConsoleAsync()
        {
            while (_isRunning)
            {
                try
                {
                    // ввод
                    string? input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input)) continue;

                    // разделение пробелом для получения аргументов
                    string[] inputParts = input.Split(' ');


                    // если команда: send
                    if (inputParts[0] == "send")
                    {
                        // переменные для отправки
                        string name = "CONSOLE"; // отправитель
                        string? recipient = null; // получатель
                        string message = inputParts[1]; // соодержание сообщения (по умолчанию 1 аргумент)

                        // аргументы
                        for (int i = 2; i < inputParts.Length; i++)
                        {
                            // аргумент для указания имени отправителя
                            if (inputParts[i] == "--to" || inputParts[i] == "-t")
                            {
                                recipient = inputParts[++i];
                                continue;
                            }
                            else
                            // аргумент для указания имени получателя
                            if (inputParts[i] == "--from" || inputParts[i] == "-f")
                            {
                                name = inputParts[++i];
                                continue;
                            }

                        }

                        // если получатели не указаны, то отправляем всем
                        if (recipient == null)
                        {
                            await BoardcastAsync(JsonSerializer.Serialize(new Protocol(
                                ServerCommand.SendMessage,
                                message,
                                name)));
                        }
                        else // если указаны то только им
                        {
                            await BoardcastAsync(JsonSerializer.Serialize(new Protocol(
                                ServerCommand.SendMessage,
                                message,
                                name)), recipient);
                        }
                    }
                    // если команда: online
                    if (inputParts[0] == "online")
                    {
                        lock (consoleOutputLocker)
                        {
                            Console.WriteLine("== Текущий онлайн: ==");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            foreach (var onlineUser in clients.Keys.ToArray())
                            {
                                Console.WriteLine(onlineUser);
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    continue;
                }

            }

        }


        // отправка сообщений участникам чата:
        // отправка всем участникам
        private async Task BoardcastAsync(string json)
        {
            if (_debug) lock (consoleOutputLocker) { Console.WriteLine($"Сообщение всем: json"); }

            foreach (var client in clients)
            {
                await client.Value.GetStreamWriter().WriteLineAsync(json);
            }
        }

        // отправка всем, кроме
        private async Task BoardcastAsync(string json, string exceptClientName)
        {
            if (_debug) lock (consoleOutputLocker) { Console.WriteLine($"Сообщение {exceptClientName}: {json}"); }

            foreach (var client in clients)
            {
                if (client.Key == exceptClientName) continue;

                await client.Value.GetStreamWriter().WriteLineAsync(json);
            }
        }

        //private async Task BoardcastAsync(string json, string[] recipientNames)

    }
}
