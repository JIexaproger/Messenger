using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        ServerOnline // strings
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
        /// Тип команды для сервера и дальнейшей обработки
        /// </summary>
        public ServerCommand Command { get; }

        /// <summary>
        /// Имя
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Текст
        /// </summary>
        public string? Message { get; }

        /// <summary>
        /// Id клиента
        /// </summary>
        public int? Id { get; }

        /// <summary>
        /// Лист строк
        /// </summary>
        public string[]? Strings { get; }

        /// <summary>
        /// Тип ошибки от сервера
        /// </summary>
        public Error Error { get; }

        /// <summary>
        /// Серверное поле в котором отправляется статус принятия сообщения
        /// </summary>
        public bool? Status { get; }

        /// <summary>
        /// Есть ли в сообщении упоминание
        /// </summary>
        public bool Mentioned { get; }



        public Protocol(
            ServerCommand command,
            string? name = null,
            string? message = null,
            Error error = 0,
            int? id = null,
            string[]? strings = null,
            bool? status = null,
            bool mentioned = false,
            int? nameColor = null)
        {
            Command = command;
            Message = message;
            Name = name;
            Error = error;
            Id = id;
            Strings = strings;
            Status = status;
            Mentioned = mentioned;
        }

        [JsonConstructor]
        public Protocol(
            ServerCommand command,
            string? name,
            string? message,
            int? id,
            string[]? strings,
            Error error,
            bool? status,
            bool mentioned)
        {
            Command = command;
            Message = message;
            Name = name;
            Id = id;
            Error = error;
            Strings = strings;
            Status = status;
            Mentioned = mentioned;
        }

        //public Protocol(string raw)
        //{
        //    string[] protokolParts = raw.Split(',');

        //    foreach (var part in protokolParts)
        //    {
        //        if (part.StartsWith("<command>:"))
        //        {
        //            if (Enum.TryParse(part.Substring(10), out ServerCommand command))
        //            {
        //                Command = command;
        //                continue;
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //        if (part.StartsWith("<message>:"))
        //        {
        //            Message = part.Substring(10);
        //            continue;
        //        }
        //        if (part.StartsWith("<name>:"))
        //        {
        //            Name = part.Substring(7);
        //            continue;
        //        }
        //        if (part.StartsWith("<status>:"))
        //        {
        //            if (part.Substring(9).ToLower() == "true")
        //            {
        //                Status = true;
        //            }
        //            else Status = false;

        //            continue;
        //        }
        //    }
        //}

        public Protocol() { }
    }
}
