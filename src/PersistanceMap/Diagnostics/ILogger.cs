using System;

namespace PersistanceMap.Diagnostics
{
    public interface ILogger
    {
        void Write(string message, string category = null, DateTime? logtime = null);
    }
}
