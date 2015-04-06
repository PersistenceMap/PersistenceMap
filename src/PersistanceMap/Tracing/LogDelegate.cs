using System;
using System.Diagnostics;

namespace PersistanceMap.Tracing
{
    /// <summary>
    /// Delegates all log entries to all the defined loggers in the loggerfactory
    /// </summary>
    public class LogDelegate : ILogger
    {
        readonly ILoggerFactory _loggerFactory;

        public LogDelegate(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Writes the logentry to all loggers defined in the loggerfactory
        /// </summary>
        /// <param name="message">The logmessage</param>
        /// <param name="source">The logsource</param>
        /// <param name="category">The logcategory</param>
        /// <param name="logtime">The time of the log</param>
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
