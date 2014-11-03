using System.Collections.Generic;

namespace PersistanceMap.Diagnostics
{
    public interface ILoggerFactory
    {
        IEnumerable<ILogger> LogProviders { get; }

        void AddLogger(string name, ILogger loggerProvider);

        ILogger CreateLogger();
    }
}
