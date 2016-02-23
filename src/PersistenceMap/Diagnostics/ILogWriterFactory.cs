using System.Collections.Generic;

namespace PersistenceMap.Diagnostics
{
    public interface ILogWriterFactory
    {
        IEnumerable<ILogWriter> LogProviders { get; }

        void AddLogger(string name, ILogWriter loggerProvider);

        ILogWriter CreateLogger();
    }
}
