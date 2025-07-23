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
            await client.ConnectAsync(Console.ReadLine(), 6969);

            Console.WriteLine("Подключен к " + client.Client.RemoteEndPoint);

            // буфер для считывания данных
            byte[] data = new byte[512];
            // получаем NetworkStream для взаимодействия с сервером
            var stream = client.GetStream();
            // получаем данные из потока
            int bytes = await stream.ReadAsync(data, 0, data.Length);
            // получаем отправленное время
            string mes = Encoding.UTF8.GetString(data, 0, bytes);
            Console.WriteLine(mes);
        }

    }
}
