using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Messanger
{
    public static class Server
    {
        public static void GetIP()
        {
            Console.WriteLine("Активные локальные IP-адреса:");

            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Исключаем виртуальные и неактивные интерфейсы
                if (netInterface.OperationalStatus != OperationalStatus.Up ||
                    netInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    continue;

                foreach (UnicastIPAddressInformation ip in netInterface.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        Console.WriteLine($"{netInterface.Name}: {ip.Address}");
                    }
                }
            }
        }

        //public void CreateServer()
        //{
        //    // 1. Создаём TCP-слушатель на всех сетевых интерфейсах (IPAddress.Any) и порту 8888
        //    TcpListener server = new TcpListener(IPAddress.Any, 8888);

        //    // 2. Запускаем сервер
        //    server.Start();
        //    Console.WriteLine("Сервер запущен. Ожидание подключения...");

        //    // 3. Ожидаем подключения клиента (блокирующий вызов)
        //    TcpClient client = server.AcceptTcpClient();
        //    Console.WriteLine("Клиент подключен!");

        //    // 4. Получаем поток для чтения/записи данных
        //    NetworkStream stream = client.GetStream();

        //    while (true)
        //    {
        //        // 5. Буфер для приёма данных (максимум 1024 байта)
        //        byte[] buffer = new byte[1024];

        //        // 6. Читаем данные из потока (сколько реально пришло — вернёт в bytesRead)
        //        int bytesRead = stream.Read(buffer, 0, buffer.Length);

        //        // 7. Декодируем байты в строку (UTF8)
        //        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        //        Console.WriteLine($"{ client.Client.RemoteEndPoint.AddressFamily.ToString()}: {message}");
        //    }
        //}
    }
}
