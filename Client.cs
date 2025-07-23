using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Messanger
{
    internal class Client
    {
        public void NewClient()
        {
            // 1. Запрашиваем IP-адрес сервера у пользователя
            Console.Write("Введите IP-адрес сервера: ");
            string serverIp = Console.ReadLine();

            // 2. Создаём TCP-клиент и подключаемся к серверу (IP + порт 8888)
            TcpClient client = new TcpClient(serverIp, 8888);

            // 3. Получаем поток для отправки данных
            NetworkStream stream = client.GetStream();

            while (true)
            {
                // 4. Запрашиваем у пользователя сообщение
                Console.Write("Введите сообщение: ");
                string message = Console.ReadLine();

                // 5. Преобразуем строку в байты (UTF8)
                byte[] data = Encoding.UTF8.GetBytes(message);

                // 6. Отправляем данные через поток
                stream.Write(data, 0, data.Length);
                Console.WriteLine("Сообщение отправлено!");
            }
        }
    }
}
