using System;

namespace PersistanceMap.Diagnostics
{
    class Logger : ILogger
    {
        readonly ILoggerFactory _loggerFactory;

        public Logger(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public void Write(string message, string category = null, DateTime? logtime = null)
        {
            //TODO: add message to _loggerFactory.LogQueue
            //TODO: find alternative to _loggerFactory.LogQueue /other place for LogQueue


            foreach (var logger in _loggerFactory.LogProviders)
            {
                logger().Write(message, category, logtime);
            }
        }
    }
}
