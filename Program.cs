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
            Console.WriteLine("Вы клиент или сервер?");
            Console.WriteLine("\n1 - сервер\n2 - клиент\n");

            bool isIncorrectComand = true;

            while (isIncorrectComand)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        await NewServer.CreateServer();
                        isIncorrectComand = false;
                        break;

                    case ConsoleKey.D2:
                        await NewClient.CreateClient();
                        isIncorrectComand = false;
                        break;
                }
            }
        }
    }
}
