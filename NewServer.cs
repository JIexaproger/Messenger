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
    public static class NewServer
    {
        public static async Task CreateServer()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 6969);

            try
            {
                server.Start();
                Console.WriteLine("Сервер запущен. Ожидание клиентов...");

                while (true)
                {
                    using TcpClient client = await server.AcceptTcpClientAsync();

                    var stream = client.GetStream();

                    DateTime date = DateTime.Now;

                    byte[] mes = Encoding.UTF8.GetBytes($"Привет, ты присоеденился к серверу {server.LocalEndpoint} в {date.Hour}:{date.Minute}:{date.Second}!");
                    await stream.WriteAsync(mes, 0, mes.Length);
                    Console.WriteLine($"Клиенту {client.Client.RemoteEndPoint} отправлены данные в {date.Hour}:{date.Minute}:{date.Second}");
                }
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
