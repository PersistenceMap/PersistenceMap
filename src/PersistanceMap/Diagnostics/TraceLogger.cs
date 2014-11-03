using System;
using System.Diagnostics;
using System.Text;

namespace PersistanceMap.Diagnostics
{
    /// <summary>
    /// ILogger class that places all logs to Trace output
    /// </summary>
    public class TraceLogger : ILogger
    {
        public void Write(string message, string source = null, string category = null, DateTime? logtime = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("#### PersistanceMap - {0}", source));
            sb.AppendLine(message.TrimEnd());
            sb.AppendLine(string.Format("## Execute at: {0}", logtime ?? DateTime.Now));
            sb.AppendLine(string.Format("## Category: {0}", category));

            Trace.WriteLine(sb.ToString());
        }
    }
}
