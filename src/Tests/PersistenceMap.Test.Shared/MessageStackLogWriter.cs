using PersistenceMap.Diagnostics;
using System;
using System.Collections.Generic;

namespace PersistenceMap.Test
{
    public class MessageStackLogWriter : ILogWriter
    {
        public MessageStackLogWriter()
        {
            Logs = new List<LogMessage>();
        }

        public IList<LogMessage> Logs { get; private set; }

        public void Write(string message, string source = null, string category = null, DateTime? logtime = null)
        {
            Logs.Add(new LogMessage
            {
                Message = message,
                Source = source,
                Category = category,
                LogTime = logtime
            });
        }
    }

    public class LogMessage
    {
        public string Message { get; set; }

        public string Source { get; set; }

        public string Category { get; set; }

        public DateTime? LogTime { get; set; }
    }
}
