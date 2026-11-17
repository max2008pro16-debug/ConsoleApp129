using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129.Core
{
    // Наш собственный класс исключения
    internal class GameException : Exception
    {
        // Код ошибки для логирования
        public string ErrorCode { get; }

        // Конструктор для создания исключения
        public GameException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        // Конструктор, если нужно передать внутреннее исключение (например, от файловой системы)
        public GameException(string message, string errorCode, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}