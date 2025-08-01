using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace messanger2
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            string mode = "";
            string ip = "0.0.0.0";
            int port = 5959;
            bool debug = false;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-s") mode = "s";
                else if (args[i] == "-c") mode = "c";
                else if (args[i] == "-d") debug = true;
                else if (args[i] == "-ip") { ip = args[i + 1]; i++; }
                else if (args[i] == "-p") { port = Convert.ToInt32(args[i + 1]); i++; }
            }

            if (mode == "s")
            {
                Server server = new Server(ip, port, debug);
                await server.CreateServer();
            }
            else if (mode == "c")
            {

            }
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
                            Server server = new Server(IPAddress.Parse(serverIp), serverPort);
                            await server.CreateServer();
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
                            //ip = Console.ReadLine();
                            //Console.Write("Введите порт сервера: ");
                            //port = Convert.ToInt32(Console.ReadLine());
                            //await Client.NewClient(ip, port);
                            //Client client = new Client("klisov.ru", 8000, true);
                            Client client = new Client();
                            await client.StartClient();
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
