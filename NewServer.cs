using System;
using System.Collections.Generic;
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
        private static List<TcpClient> clients = new List<TcpClient>(); // список всех клиентов

        public static async Task CreateServer()
        {
            bool isIncorrectPort = true; // переменная для корректного вода порта
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

                    clients.Add(client);

                    Task.Run(async () => await ListingClientAsync(client));

                }
            }
            finally
            {
                server.Stop();
            }
        }

        private static async Task ListingClientAsync(TcpClient client)
        {
            var stream = client.GetStream();

            var response = new List<byte>();
            int byteRead = '\n';

            while(true)
            {
                response.Clear();

                while ((byteRead = stream.ReadByte()) != '\n')
                {
                    response.Add((byte)byteRead);
                }

                string fullResponse = Encoding.UTF8.GetString(response.ToArray());

                if (fullResponse == "END") break;

                string author = Convert.ToString(client.Client.RemoteEndPoint);

                Console.WriteLine($"Клиент {client.Client.RemoteEndPoint} написал: {fullResponse}");

                await SendingClientsAsync(clients.ToArray(), fullResponse, author);

            }

            client.Close();
        }

        private static async Task SendingClientsAsync(TcpClient[] clients, string mes, string author)
        {
            foreach (var client in clients)
            {
                var clientStream = client.GetStream();

                string mes1 = $"{author}: {mes}" + '\n';

                await clientStream.WriteAsync(Encoding.UTF8.GetBytes(mes1), 0, mes1.Length);
                Console.WriteLine($" > Клиенту {client.Client.RemoteEndPoint} отправленно сообщение от {author}");
            }
        }
    }
}
