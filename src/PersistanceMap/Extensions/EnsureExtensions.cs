using System;

namespace PersistanceMap
{
    internal static class EnsureExtensions
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

        public static void EnsureArgumentNotNullOrEmpty(this string argument, string name, string message)
        {
            if (String.IsNullOrEmpty(argument))
                throw new ArgumentException(message ?? "Cannot be null or empty", name);
        }
    }
}
