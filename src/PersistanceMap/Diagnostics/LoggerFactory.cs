﻿using System.Collections.Generic;
using System.Diagnostics;

namespace PersistanceMap.Diagnostics
{
    public delegate ILogger CreateLoggerCallback();

    public class LoggerFactory : ILoggerFactory
    {
        readonly Dictionary<string, CreateLoggerCallback> _logProviders = new Dictionary<string, CreateLoggerCallback>();
        public IEnumerable<CreateLoggerCallback> LogProviders
        {
            get
            {
                return _logProviders.Values;
            }
        }

        public void AddLogger(string name, CreateLoggerCallback loggerProvider)
        {
            if (!_logProviders.ContainsKey(name))
            {
                _logProviders.Add(name, loggerProvider);

                Trace.WriteLine(string.Format("#### PersistanceMap - Added Logger: {0}", name));
            }
        }

        public ILogger CreateLogger()
        {
            return new Logger(this);
        }
    }
}