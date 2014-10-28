using System.Diagnostics;

namespace PersistanceMap
{
    internal static class Logger
    {
        public static void Write(string message, params object[] args)
        {
            Trace.WriteLine(string.Format("### PersistanceMap: {0}", string.Format(message, args)), "PersistanceMap");
        }

        public static void Write(string message)
        {
            Trace.WriteLine(string.Format("### PersistanceMap: {0}", message), "PersistanceMap");
        }

        public static void Write(object message)
        {
            Trace.WriteLine(string.Format("### PersistanceMap: {0}", message), "PersistanceMap");
        }
    }
}
