using System;

namespace PersistenceMap.Diagnostics
{
    public interface ILogWriter
    {
        void Write(string message, string source = null, string category = null, DateTime? logtime = null);
    }
}
