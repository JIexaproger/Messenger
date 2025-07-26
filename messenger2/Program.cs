using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace messanger1
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
            if (mode == "s")
            {
                await Server.CreateServer(ip, port);
            }
            else if (mode == "c")
            {
                await Client.NewClient(ip, port);
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
                            Console.Write("Введите IP сервера: ");
                            ip = Console.ReadLine();
                            Console.Write("Введите порт сервера: ");
                            port = Convert.ToInt32(Console.ReadLine());
                            await Client.NewClient(ip, port);
                            //await Client.NewClient("192.168.0.12", 5959);
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
