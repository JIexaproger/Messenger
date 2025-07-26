//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;

//namespace Messanger
//{
//    public class NewClient
//    {
//        public static async Task CreateClient()
//        {
//        //    try
//        //    {
//        //        using TcpClient client = new TcpClient();

//        //        Console.Write("\nВведите IP сервера: ");
//        //        //await client.ConnectAsync(Console.ReadLine(), 6969);
//        //        await client.ConnectAsync("192.168.0.12", 6969);

//        //        Console.WriteLine("Подключен к " + client.Client.RemoteEndPoint);

//        //        var stream = client.GetStream();
//        //        using var reader = new StreamReader(stream);
//        //        using var writer = new StreamWriter(stream);

//        //        Console.Write("Введите ваше имя: ");
//        //        await WriteMessage(writer, Console.ReadLine());


//        //        var receiveTask = ReceiveMessagesAsync(reader);


//        //        while (true)
//        //        {
//        //            Console.Write("> ");
//        //            await WriteMessage(writer, Console.ReadLine());
//        //        }
//        //    }
//        //}

//        //public static async Task ReceiveMessagesAsync(StreamReader reader)
//        //{
//        //    string response = await reader.ReadToEndAsync();

//        //    Console.WriteLine(response);
//        //    Console.Write("> ");
//        //}

//        //public static async Task WriteMessage(StreamWriter stream, string mes)
//        //{

//        //    await stream.FlushAsync();
//        //}
//    }
//}