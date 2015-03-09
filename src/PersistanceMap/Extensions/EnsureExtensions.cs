using System;

namespace PersistanceMap
{
    public static class EnsureExtensions
    {
        public static void EnsureArgumentNotNull(this object argument, string name)
        {
            if (argument == null)
                throw new ArgumentNullException(name, "Cannot be null");
        }

        public static void EnsureArgumentNotNullOrEmpty(this string argument, string name)
        {
            if (String.IsNullOrEmpty(argument))
                throw new ArgumentException("Cannot be null or empty", name);
        }
    }
}
