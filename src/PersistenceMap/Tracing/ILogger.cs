using System;

namespace PersistenceMap.Tracing
{
    public interface ILogger
    {
        void Write(string message, string source = null, string category = null, DateTime? logtime = null);
    }
}
