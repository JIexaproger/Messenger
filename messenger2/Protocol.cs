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

        /// <summary>
        /// Лист строк
        /// </summary>
        public string[]? Strings { get; }

        /// <summary>
        /// Серверное поле в котором отправляется статус принятия сообщения
        /// </summary>
        public bool? Status { get; }

        /// <summary>
        /// Есть ли в сообщении упоминание
        /// </summary>
        public bool Mentioned { get; } = false;




        public Protocol(
            ServerCommand command,
            string? name = null,
            string? message = null,
            Error error = 0,
            string[]? strings = null,
            bool? status = null,
            bool mentioned = false)
        {
            Command = command;
            Message = message;
            Name = name;
            Error = error;
            Strings = strings;
            Status = status;
            Mentioned = mentioned;
        }
    }
}
