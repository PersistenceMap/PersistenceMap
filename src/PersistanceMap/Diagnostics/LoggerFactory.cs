using System.Collections.Generic;
using System.Diagnostics;

namespace PersistanceMap.Diagnostics
{
    //public delegate ILogger CreateLoggerCallback();

    public class LoggerFactory : ILoggerFactory
    {
        readonly Dictionary<string, ILogger> _logProviders = new Dictionary<string, ILogger>();
        public IEnumerable<ILogger> LogProviders
        {
            get
            {
                return _logProviders.Values;
            }
        }

        public void AddLogger(string name, ILogger logger)
        {
            if (!_logProviders.ContainsKey(name))
            {
                _logProviders.Add(name, logger);

                Trace.WriteLine(string.Format("#### PersistanceMap - Added Logger: {0}", name));
            }
        }

        public ILogger CreateLogger()
        {
            return new Logger(this);
        }
    }
}
