using System;

namespace PersistanceMap
{
    public class InvalidConverterException : Exception
    {
        public InvalidConverterException(string message, Exception baseException)
            : base(message, baseException)
        {
        }
    }
}
