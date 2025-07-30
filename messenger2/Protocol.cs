using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace messanger2
{
    public enum ServerCommand
    {
        NotInit,
        VerifiedLogin, // status, message*
        UserLogin, // name           
        UserJoin, // name
        UserLeave, // name
        UserMention, // name
        SendMessage, // message, mentioned
        SendCommand, // message, 
        ServerOnline // name, message
    }

    public enum Error
    {
        NotInit,
        NameTooShort,
        NameTooLong,
        NameIsEmpty,
        NameNotAvailable,
        UserNotFound
    }

    class Protocol
    {
        /// <summary>
        /// Текст
        /// </summary>
        public string? Message { get; }

        /// <summary>
        /// Имя
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Тип команды для сервера и дальнейшей обработки
        /// </summary>
        public ServerCommand Command { get; }

        /// <summary>
        /// Тип ошибки от сервера
        /// </summary>
        public Error Error { get; }

        public List<string>? Strings { get; }

        /// <summary>
        /// Разбиение сообщения на части для цветного форматирования
        /// </summary>
        //public Dictionary<string, ConsoleColor> MessageParts { get; }

        /// <summary>
        /// Серверное поле в котором отправляется статус принятия сообщения
        /// </summary>
        public bool? Status { get; }

        /// <summary>
        /// Есть ли в сообщении упоминание
        /// </summary>
        public bool? Mentioned { get; }



        public Protocol(string raw)
        {
            string[] protokolParts = raw.Split(';');

            foreach (var part in protokolParts)
            {
                if (part.StartsWith("<command>:"))
                {
                    if (Enum.TryParse(part.Substring(10), out ServerCommand command))
                    {
                        Command = command;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                if (part.StartsWith("<error>:"))
                {
                    if (Enum.TryParse(part.Substring(10), out Error error))
                    {
                        Error = error;
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (part.StartsWith("<message>:"))
                {
                    if (!string.IsNullOrEmpty(part.Substring(10)))
                    {
                        Message = part.Substring(10);
                    }
                    continue;
                }
                if (part.StartsWith("<name>:"))
                {
                    if (!string.IsNullOrEmpty(part.Substring(10)))
                    {
                        Name = part.Substring(7);
                    }
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
                if (part.StartsWith("<mentioned>:"))
                {
                    if (part.Substring(12).ToLower() == "true")
                    {
                        Mentioned = true;
                    } 
                    else Mentioned = false;

                    continue;
                }
            }
        }

        public static string BuildProtocolString(
            ServerCommand serverCommand,
            string? name = null,
            string? message = null,
            string[]? strings = null,
            Error error = 0,
            bool? status = null,
            bool? mentioned = null)
        {
            var protokolParts = new List<string>();

            protokolParts.Add($"<command>:{serverCommand}");

            if (name != null) protokolParts.Add($"<name>:{name}");
            if (message != null) protokolParts.Add($"<message>:{message}");
            if (strings != null) protokolParts.Add($"<strings>:{message}");
            if (error != 0) protokolParts.Add($"<error>:{error}");
            if (status != null) protokolParts.Add($"<status>:{status}");
            if (mentioned != null) protokolParts.Add($"<mentioned>:{mentioned}");

            return string.Join(";", protokolParts.ToArray());
        }


        public static string BuildProtocolString(Protocol protocol)
        {
            var protokolParts = new List<string>();

            protokolParts.Add($"<command>:{protocol.Command}");

            if (protocol.Name != null) protokolParts.Add($"<name>:{protocol.Name}");
            if (protocol.Message != null) protokolParts.Add($"<message>:{protocol.Message}");
            if (protocol.Error != 0) protokolParts.Add($"<error>:{protocol.Error}");
            if (protocol.Status != null) protokolParts.Add($"<status>:{protocol.Message}");
            if (protocol.Mentioned != null) protokolParts.Add($"<mentioned>:{protocol.Mentioned}");

            return string.Join(";", protokolParts.ToArray());
        }

    }
}
