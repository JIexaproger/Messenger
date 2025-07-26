using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace Messanger
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            //Protocol protocol = new Protocol("<command>:UserLeave;<status>:false;<message>:ляля25959;<name>:хс");

            //Console.WriteLine(protocol.Name);
            //Console.WriteLine(protocol.Command);
            //Console.WriteLine(protocol.Message);
            //Console.WriteLine(protocol.Status);

            //Protocol protocol1 = new Protocol(Protocol.BuildProtocolString(Protocol.ServerCommands.UserJoin, false, message: "kiruhaGPT"));

            //Console.WriteLine(protocol1.Name);
            //Console.WriteLine(protocol1.Command);
            //Console.WriteLine(protocol1.Message);
            //Console.WriteLine(protocol1.Status);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Вы клиент или сервер?");
                Console.WriteLine("\n1 - сервер\n2 - клиент\n");

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        try
                        {
                            Console.Write("Введите доступные IP для подключения к серверу (оставте пустым для любых IP (для публичных чатов)): ");
                            string serverIp = Console.ReadLine();
                            if (serverIp == "" || serverIp == "0") { serverIp = "0.0.0.0"; }
                            Console.Write("Введите порт сервера: ");
                            int serverPort = Convert.ToInt32(Console.ReadLine());
                            await Server.CreateServer(serverIp, serverPort);
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Произошла ошибка: " + ex);
                        }
                        break;

                    case ConsoleKey.D2:
                        try
                        {
                            //Console.Write("Введите IP сервера: ");
                            //string ip = Console.ReadLine();
                            //Console.Write("Введите порт сервера: ");
                            //int port = Convert.ToInt32(Console.ReadLine());
                            //await Client.NewClient(ip, port);
                            await Client.NewClient("192.168.0.12", 5959);
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Произошла ошибка: " + ex);
                        }
                        break;
                }
            }
        }
    }
}
