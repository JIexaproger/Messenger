using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;


namespace Messanger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Вы клиент или сервер?");
            Console.WriteLine("\n1 - клиент\n2 - сервер\n");

            bool isCorrectComand = true;

            while (isCorrectComand)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        
                        isCorrectComand = true;
                        break;

                    case ConsoleKey.D2:
                        NewServer.GetIP();

                        isCorrectComand = true;
                        break;
                }
            }


        }
    }
}
