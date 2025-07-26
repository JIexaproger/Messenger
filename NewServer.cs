using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Messanger
{
    public class NewServer
    {
        private static Dictionary<TcpClient, string> clients = new Dictionary<TcpClient, string>(); // список всех клиентов


        public static async Task CreateServer()
        {
            //bool isIncorrectPort = true; // переменная для корректного вода порта
            int port = 6969; // инициализация порта с дефолтным значением 6969
            //while (isIncorrectPort)
            //{
            //    try
            //    {
            //        Console.Write("Введите порт сервера: ");
            //        port = Convert.ToInt32(Console.ReadLine());
            //        if (!(0 <= port) || !(port < 65536))
            //        {
            //            Console.WriteLine("Ошибка! Введите число порта от 1 до 65535 (пример: 1234, 1, 65535)\n");
            //        } 
            //        else isIncorrectPort = false;
            //    } 
            //    catch (FormatException) 
            //    {
            //        Console.WriteLine("Ошибка! Введите число порта от 1 до 65535 (пример: 1234, 1, 65535)\n");
            //    }
            //}

            TcpListener server = new TcpListener(IPAddress.Any, port);

            try
            {
                server.Start();
                Console.WriteLine("Сервер запущен с параметрами: ");

                Console.WriteLine("Порт сервера: " + ((IPEndPoint)server.LocalEndpoint).Port);

                while (true)
                {

                    var client = await server.AcceptTcpClientAsync();

                    var streamReader = new StreamReader(client.GetStream());
                    string clientName = await streamReader.ReadLineAsync();

                    clients.Add(client, clientName);

                    Task.Run(async () => await ProcessClientAsync(client, clientName));

                }
            }
            finally
            {
                server.Stop();
            }
        }

        private static async Task ProcessClientAsync(TcpClient client, string clientName)
        {
            Console.WriteLine(clientName + " подключился.");
            var stream = client.GetStream();

            using var streamReader = new StreamReader(stream);
            using var streamWriter = new StreamWriter(stream);

            while(true)
            {
                string response = await streamReader.ReadLineAsync();

                Console.WriteLine($"Клиент {clientName} написал: {response}");
                await SendAll(client, clientName, response);
            }
        }

        public static async Task SendAll(TcpClient client, string clientName, string message)
        {
            foreach (var item in clients)
            {
                if (item.Key != client)
                {
                    var stream = new StreamWriter(item.Key.GetStream());
                    Console.WriteLine("otpravlay " + item.Value);
                    await WriteMessage(stream, clientName + " : " + message);
                    Console.WriteLine("otpraviy");
                }
            }
        }

        public static async Task WriteMessage(StreamWriter stream, string mes)
        {
            await stream.WriteAsync(mes);
            await stream.FlushAsync();
        }
    }
}
