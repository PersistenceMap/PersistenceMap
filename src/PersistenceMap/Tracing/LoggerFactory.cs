using System.Collections.Generic;
using System.Diagnostics;

namespace PersistenceMap.Tracing
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

                //Trace.WriteLine(string.Format("## PersistenceMap - Added Logger: {0}", name));
            }
        }

        public ILogger CreateLogger()
        {
            return new LogDelegate(this);
        }
    }
}
