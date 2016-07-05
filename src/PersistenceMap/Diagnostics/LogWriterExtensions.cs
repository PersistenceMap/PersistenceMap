using System;

namespace PersistenceMap.Diagnostics
{
    public static class LogWriterExtensions
    {
        public static ILogWriter Write(this ILogWriter logger, string message, TimeLogger timer, string source = null, string category = null, DateTime? logtime = null)
        {
            logger.Write($"{message}{Environment.NewLine}{timer.ToString()}".TrimEnd(), source, category, logtime);

            return logger;
        }
    }
}
