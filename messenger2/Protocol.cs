using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messanger1
{
    public enum ServerCommands
    {
        NotInit,
        VerifiedLogin, // status, message*
        UserLogin, // name           
        UserJoin, // name
        UserLeave, // name
        SendMessage // name, message
    }

    class Protocol
    {
        /// <summary>
        /// Текст первичного сообщения
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Имя отправителя первичного протокола
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Тип команды для сервера и дальнейшей обработки
        /// </summary>
        public ServerCommands Command { get; }

        /// <summary>
        /// Разбиение сообщения на части для цветного форматирования
        /// </summary>
        public Dictionary<string, ConsoleColor> MessageParts { get; }

        /// <summary>
        /// Серверное поле в котором отправляется статус принятия сообщения
        /// </summary>
        public bool Status { get; }


        


        public Protocol(string raw)
        {
            string[] protokolParts = raw.Split(';');

            foreach (var part in protokolParts)
            {
                if (part.StartsWith("<command>:"))
                {
                    if (Enum.TryParse(part.Substring(10), out ServerCommands command))
                    {
                        Command = command;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                if (part.StartsWith("<message>:"))
                {
                    Message = part.Substring(10);
                    continue;
                }
                if (part.StartsWith("<name>:"))
                {
                    Name = part.Substring(7);
                    continue;
                }
                if (part.StartsWith("<status>:"))
                {
                    if (part.Substring(9).ToLower() == "true")
                    {
                        Status = true;
                    } 
                    else Status = false;

                    continue;
                }
            }
        }

        public static string BuildProtocolString(ServerCommands serverCommands, string name = null, string message = null)
        {
            var protokolParts = new List<string>();
            protokolParts.Add($"<command>:{serverCommands}");
            if (name != null) protokolParts.Add($"<name>:{name}");
            if (message != null) protokolParts.Add($"<message>:{message}");

            return string.Join(";", protokolParts.ToArray());
        }

        public static string BuildProtocolString(ServerCommands serverCommands, bool status, string name = null, string message = null)
        {
            var protokolParts = new List<string>();
            protokolParts.Add($"<command>:{serverCommands}");
            if (name != null) protokolParts.Add($"<name>:{name}");
            if (message != null) protokolParts.Add($"<message>:{message}");
            protokolParts.Add($"<status>:{status}");

            return string.Join(";", protokolParts.ToArray());
        }
    }
}
