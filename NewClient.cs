using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Messanger
{
    public class NewClient
    {
        public static async Task CreateClient()
        { 
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await socket.ConnectAsync("192.168.0.12", 6969);
                Console.WriteLine("Подключен к " + socket.RemoteEndPoint);
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Возникла ошибка {e.Message} при подключении к {socket.RemoteEndPoint}");
            }

            socket.Close();
        }

    }
}
