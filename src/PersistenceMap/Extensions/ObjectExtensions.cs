using System;

namespace PersistenceMap
{
    internal static class ObjectExtensions
    {
        public static bool IsDBNull(this object obj)
        {
            return obj is DBNull;
        }
    }
}
