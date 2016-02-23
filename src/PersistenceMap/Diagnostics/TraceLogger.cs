using System;
using System.Diagnostics;
using System.Text;

namespace PersistenceMap.Diagnostics
{
    /// <summary>
    /// ILogger class that formats and places all logs to Trace output
    /// </summary>
    public class TraceLogger : ILogWriter
    {
        public void Write(string message, string source = null, string category = null, DateTime? logtime = null)
        {
            var sb = new StringBuilder();

            switch (category)
            {
                case LoggerCategory.Query:
                    sb.AppendLine(string.Format("PersistenceMap - {0}", source));
                    sb.AppendLine(message.TrimEnd());
                    AppendCategory(sb, category);
                    AppendTime(sb, logtime);
                    AppendSource(sb, source);
                    break;

                default:
                    sb.AppendLine(string.Format("PersistenceMap - {0}", message.TrimEnd()));
                    AppendCategory(sb, category);
                    AppendTime(sb, logtime);
                    AppendSource(sb, source);
                    break;
            }

            switch (category)
            {
                case LoggerCategory.Error:
                    Trace.TraceError(sb.ToString());
                    break;

                case LoggerCategory.Query:
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

            sb.AppendLine(string.Format("## Category: {0}", category));
        }

        private static void AppendSource(StringBuilder sb, string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return;
            }

            sb.AppendLine(string.Format("## Source: {0}", source));
        }

        private static void AppendTime(StringBuilder sb, DateTime? logtime)
        {
            if (logtime == null)
            {
                return;
            }

            sb.AppendLine(string.Format("## Execute at: {0}", logtime ?? DateTime.Now));
        }
    }
}
