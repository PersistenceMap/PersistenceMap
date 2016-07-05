using Scribe;
using System;
using System.Diagnostics;
using System.Text;

namespace PersistenceMap.Samples
{
    public static class ScribeExtensions
    {
        public static LoggerConfiguration AddPersistenceMapTraceWriter(this LoggerConfiguration configuration)
        {
            configuration.AddWriter(new TraceLogger());

            return configuration;
        }
    }

    public class PersistenceMapLogListener : IListener, PersistenceMap.Diagnostics.ILogWriter
    {
        private ILogger _logger;

        public void Initialize(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.GetLogger();
        }

        public void Write(string message, string source = null, string category = null, DateTime? logtime = null)
        {
            if (_logger == null)
            {
                return;
            }

            _logger.Write(message, LogLevel.Information, Priority.Medium, category, logtime);
        }
    }

    public class TraceLogger : Scribe.ILogWriter
    {
        public void Write(ILogEntry logEntry)
        {
            var sb = new StringBuilder();

            switch (logEntry.Category)
            {
                case Diagnostics.LoggerCategory.Query:
                    sb.AppendLine($"PersistenceMap - Query");
                    sb.AppendLine(logEntry.Message.TrimEnd());
                    AppendCategory(sb, logEntry.Category);
                    AppendTime(sb, logEntry.LogTime);
                    //AppendSource(sb, logEntry.source);
                    break;

                default:
                    sb.AppendLine($"PersistenceMap - {logEntry.Category}");
                    sb.AppendLine(logEntry.Message.TrimEnd());
                    AppendCategory(sb, logEntry.Category);
                    AppendTime(sb, logEntry.LogTime);
                    //AppendSource(sb, source);
                    break;
            }

            switch (logEntry.Category)
            {
                case Diagnostics.LoggerCategory.Error:
                    Trace.TraceError(sb.ToString());
                    break;

                case Diagnostics.LoggerCategory.Query:
                    Trace.WriteLine(sb.ToString());
                    break;

                default:
                    Trace.WriteLine(sb.ToString());
                    break;
            }
        }

        private static void AppendCategory(StringBuilder sb, string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                return;
            }

            sb.AppendLine($"## Category: {category}");
        }

        private static void AppendTime(StringBuilder sb, DateTime? logtime)
        {
            if (logtime == null)
            {
                return;
            }

            sb.AppendLine($"## Execute at: {logtime ?? DateTime.Now}");
        }
    }
}
