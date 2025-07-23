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
            Console.WriteLine("\n1 - клиент\n2 - сервер\n");

            bool isCorrectComand = true;

            while (isCorrectComand)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        await NewClient.CreateClient();
                        isCorrectComand = false;
                        break;

                    case ConsoleKey.D2:
                        await NewServer.CreateServer();
                        isCorrectComand = false;
                        break;
                }
            }
        }
    }
}
