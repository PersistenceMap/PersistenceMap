using System;

namespace PersistanceMap.Diagnostics
{
    public interface ILogger
    {
        void Write(string message, string source = null, string category = null, DateTime? logtime = null);
    }
}
