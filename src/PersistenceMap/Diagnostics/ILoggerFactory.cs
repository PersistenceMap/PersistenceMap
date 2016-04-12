using System.Collections.Generic;

namespace PersistenceMap.Diagnostics
{
    public interface ILoggerFactory
    {
        IEnumerable<ILogWriter> LogProviders { get; }

        void AddWriter(string name, ILogWriter loggerProvider);

        ILogWriter CreateLogger();
    }
}
