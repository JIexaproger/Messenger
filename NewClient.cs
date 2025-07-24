using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Messanger
{
    public class NewClient
    {
        public static async Task CreateClient()
        {
            using TcpClient client = new TcpClient();

            Console.Write("\nВведите IP сервера: ");
            //await client.ConnectAsync(Console.ReadLine(), 6969);
            await client.ConnectAsync("192.168.0.12", 6969);

            Console.WriteLine("Подключен к " + client.Client.RemoteEndPoint);

            var stream = client.GetStream();

            Task.Run(async () => await GetMessage(stream));

            while (true)
            {
                Console.WriteLine("writing pls");
                string userInput = Console.ReadLine();
                byte[] data = Encoding.UTF8.GetBytes(userInput + '\n');

                await stream.WriteAsync(data, 0, data.Length);

                if (userInput == "END") break;
            }
        }

        public static async Task GetMessage(NetworkStream stream)
        {
            var response = new List<Byte>();
            int byteRead = '\n';

            Console.WriteLine("getting");
            while (true)
            {
                response.Clear();
                Console.WriteLine("waiting");
                while ((byteRead = stream.ReadByte()) != '\n')
                {
                    response.Add((byte)byteRead);
                }

                var fullResponse = Encoding.UTF8.GetString(response.ToArray());

                Console.WriteLine(fullResponse);
            }
        }
    }
}