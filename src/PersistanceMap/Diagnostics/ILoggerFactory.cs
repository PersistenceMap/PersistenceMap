using System.Collections.Generic;

namespace PersistanceMap.Diagnostics
{
    public interface ILoggerFactory
    {
        IEnumerable<CreateLoggerCallback> LogProviders { get; }

        void AddLogger(string name, CreateLoggerCallback loggerProvider);

        ILogger CreateLogger();
    }
}
