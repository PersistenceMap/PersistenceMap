using System;

namespace PersistanceMap
{
    public static class Ensure
    {
        public static void ArgumentNotNull(this object argument, string name)
        {
            if (argument == null)
                throw new ArgumentNullException(name, "Cannot be null");
        }

        public static void ArgumentNotNullOrEmpty(this string argument, string name)
        {
            if (String.IsNullOrEmpty(argument))
                throw new ArgumentException("Cannot be null or empty", name);
        }
    }
}
