using System;
using System.Diagnostics;

namespace PersistanceMap.Diagnostics
{
    class Logger : ILogger
    {
        readonly ILoggerFactory _loggerFactory;

        public Logger(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public void Write(string message, string source = null, string category = null, DateTime? logtime = null)
        {
            // set message to all registered loggers
            foreach (var logger in _loggerFactory.LogProviders)
            {
                logger.Write(message, source, category, logtime);
            }
        }

        internal static void TraceLine(string message)
        {
            Trace.WriteLine(message);
        }

        internal static void TraceLine(string message, params string[] args)
        {
            Trace.WriteLine(string.Format(message, args));
        }
    }
}
