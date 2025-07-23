using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Messanger
{
    public class NewServer
    {
        public static async Task CreateServer()
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 6969);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipPoint);
            socket.Listen(1000);
            Console.WriteLine("Сервер запущен. Ожидание клиентов...");

            Socket client = await socket.AcceptAsync();

            Console.WriteLine($"Адрес клиента: {client.RemoteEndPoint}");

            socket.Close();
            client.Close();
        }
    }
}
