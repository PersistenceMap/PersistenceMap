using System.Collections.Generic;

namespace PersistenceMap.Tracing
{
    public interface ILoggerFactory
    {
        IEnumerable<ILogger> LogProviders { get; }

        void AddLogger(string name, ILogger loggerProvider);

        ILogger CreateLogger();
    }
}
