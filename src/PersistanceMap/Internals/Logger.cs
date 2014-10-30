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

        /// <summary>
        /// placeholder to log stuff that only gets published with logviewers
        /// </summary>
        /// <param name="message"></param>
        internal static void WriteInternal(string message)
        {
            // placeholder to log stuff that only gets published with logviewers
            //TODO: Write more detail to internal traces (Time, duration...) http://msdn.microsoft.com/en-us/data/dn469464.aspx
            //TODO: only trace to logviewer/interceptor
            Write(message);
        }
    }
}
