using System;

namespace PersistenceMap
{
    public class InvalidConverterException : Exception
    {
        public InvalidConverterException(string message, Exception baseException)
            : base(message, baseException)
        {
        }
    }
}
