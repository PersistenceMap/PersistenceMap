using System.Collections.Generic;

namespace PersistenceMap.Diagnostics
{
    public class LoggerFactory : ILoggerFactory
    {
        private readonly Dictionary<string, ILogWriter> _logProviders = new Dictionary<string, ILogWriter>();

        public IEnumerable<ILogWriter> LogProviders
        {
            get
            {
                return _logProviders.Values;
            }
        }

        public void AddWriter(string name, ILogWriter logger)
        {
            if (!_logProviders.ContainsKey(name))
            {
                _logProviders.Add(name, logger);
            }
        }

        public ILogWriter CreateLogger()
        {
            return new LogDelegate(this);
        }
    }
}
