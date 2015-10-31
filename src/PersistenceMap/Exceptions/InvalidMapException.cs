using System;

namespace PersistenceMap
{
    public class InvalidMapException : Exception
    {
        public InvalidMapException(string message, Type type, string field) 
            : base(message)
        {
            Type = type;
            Field = field;
        }

        public string Field { get; private set; }

        public Type Type { get; private set; }
    }
}
