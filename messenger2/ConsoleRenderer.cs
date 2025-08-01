using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messanger2
{

    public class ConsoleRenderer
    {
        public enum Color
        {
            // Базовые 8 цветов
            Black,
            Red,
            OrangeRed,
            Orange,
            Green,
            SeaGreen,
            Yellow,
            Blue,
            Magenta,
            Cyan,
            Violet,
            White,

            // Светлые версии
            LightBlack,
            LightRed,
            LightOrange,
            LightOrangeRed,
            LightGreen,
            LightSeaGreen,
            LightYellow,
            LightBlue,
            LightMagenta,
            LightCyan,
            LightViolet,
            LightWhite,

            Reset
        }

        public static readonly string[] ColorCode = new[]
        {
        // Базовые цвета (30-37)
        "\x1b[30m",                    // Black
        "\x1b[31m",                    // Red
        "\x1b[38;2;255;69;0m",         // OrangeRed
        "\x1b[38;2;255;152;46m",       // Orange
        "\x1b[32m",                    // Green
        "\x1b[38;2;46;139;87m",        // SeaGreen
        "\x1b[33m",                    // Yellow
        "\x1b[34m",                    // Blue
        "\x1b[35m",                    // Magenta
        "\x1b[36m",                    // Cyan
        "\x1b[38;2;238;130;238m",      // Violet
        "\x1b[37m",                    // White
        
        // Светлые цвета (90-97)
        "\x1b[38;2;128;128;128m",      // LightBlack
        "\x1b[91m",                    // LightRed
        "\x1b[38;2;255;105;46m",       // LightOrangeRed
        "\x1b[38;2;255;183;92m",       // LightOrange
        "\x1b[92m",                    // LightGreen
        "\x1b[38;2;60;179;130m",       // LightSeaGreen
        "\x1b[93m",                    // LightYellow
        "\x1b[94m",                    // LightBlue
        "\x1b[95m",                    // LightMagenta
        "\x1b[96m",                    // LightCyan
        "\x1b[38;2;221;160;221m",      // LightViolet
        "\x1b[97m",                    // LightWhite

        // Вспомогательные цвета
        "\x1b[0m"
        };



        public static string Repeat(string text, int count)
        {
            return string.Concat(Enumerable.Repeat(text, count));
        }


        public static void MessageRender(string time, string name, Color nameColor, string message)
        {
            Console.WriteLine($"{ColorCode[(int)Color.Reset]}{ColorCode[(int)Color.LightBlack]}{time}  {ColorCode[(int)Color.Reset]}<{ColorCode[(int)nameColor]}{name}{ColorCode[(int)Color.Reset]}> : {message}");
        }

        public static void test()
        {
            Console.WriteLine(
    $"{ColorCode[0]}Black{ColorCode[^1]}\n" +                     // Black
    $"{ColorCode[1]}Red{ColorCode[^1]}\n" +                       // Red
    $"{ColorCode[2]}OrangeRed{ColorCode[^1]}\n" +                  // OrangeRed
    $"{ColorCode[3]}Orange{ColorCode[^1]}\n" +                     // Orange
    $"{ColorCode[4]}Green{ColorCode[^1]}\n" +                      // Green
    $"{ColorCode[5]}SeaGreen{ColorCode[^1]}\n" +                   // SeaGreen
    $"{ColorCode[6]}Yellow{ColorCode[^1]}\n" +                     // Yellow
    $"{ColorCode[7]}Blue{ColorCode[^1]}\n" +                       // Blue
    $"{ColorCode[8]}Magenta{ColorCode[^1]}\n" +                    // Magenta
    $"{ColorCode[9]}Cyan{ColorCode[^1]}\n" +                       // Cyan
    $"{ColorCode[10]}Violet{ColorCode[^1]}\n" +                    // Violet
    $"{ColorCode[11]}White{ColorCode[^1]}\n" +                     // White
    $"{ColorCode[12]}LightBlack{ColorCode[^1]}\n" +                // LightBlack
    $"{ColorCode[13]}LightRed{ColorCode[^1]}\n" +                  // LightRed
    $"{ColorCode[14]}LightOrangeRed{ColorCode[^1]}\n" +            // LightOrangeRed
    $"{ColorCode[15]}LightOrange{ColorCode[^1]}\n" +               // LightOrange
    $"{ColorCode[16]}LightGreen{ColorCode[^1]}\n" +                // LightGreen
    $"{ColorCode[17]}LightSeaGreen{ColorCode[^1]}\n" +             // LightSeaGreen
    $"{ColorCode[18]}LightYellow{ColorCode[^1]}\n" +               // LightYellow
    $"{ColorCode[19]}LightBlue{ColorCode[^1]}\n" +                 // LightBlue
    $"{ColorCode[20]}LightMagenta{ColorCode[^1]}\n" +              // LightMagenta
    $"{ColorCode[21]}LightCyan{ColorCode[^1]}\n" +                 // LightCyan
    $"{ColorCode[22]}LightViolet{ColorCode[^1]}\n" +               // LightViolet
    $"{ColorCode[23]}LightWhite{ColorCode[^1]}");                   // LightWhite
        }




        //public static void WindowRender(int width, int height, int x, int y, string name, string message, string time, string nameColor)
        //{
        //    int windowHeight = Math.Max(name.Length + message.Length + 6, width);

        //    Console.Write(ConsoleRenderer.Colors["light_blue"]);
        //    Console.Write("╔");
        //    Console.Write(Repeat("═", windowHeight - 2));
        //    Console.Write("╗\n");
        //    for (int i = 0; i < height - 2; i++)
        //    {
        //        Console.Write("║");
        //        Console.Write($" {ConsoleRenderer.Colors[nameColor]}{name}{ConsoleRenderer.Colors["reset"]}: {message}{ConsoleRenderer.Colors["light_blue"]}");
        //        Console.SetCursorPosition(windowHeight - 1, Console.CursorTop);
        //        Console.Write("║\n");
        //    }
        //    Console.Write("╚");
        //    Console.Write(Repeat("═", windowHeight - 2));
        //    Console.Write("╝\n");
        //}
    }

    public static class StringColorExtensions
    {
        public static string Color(this string text, ConsoleRenderer.Color color)
        {
            string colorCode = ConsoleRenderer.ColorCode[(int)color];
            string resetCode = ConsoleRenderer.ColorCode[(int)ConsoleRenderer.Color.Reset];

            return $"{colorCode}{text}{resetCode}";
        }
    }
}
