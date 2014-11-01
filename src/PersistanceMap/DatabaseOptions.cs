using PersistanceMap.Configuration;
using PersistanceMap.Diagnostics;
using PersistanceMap.Internals;
using System;
using System.Configuration;

namespace PersistanceMap
{
    public class DatabaseOptions
    {
        public DatabaseOptions()
        {
            LoggerFactory = new LoggerFactory();

            // add loggers from configurationsection in app.config
            var section = ConfigurationManager.GetSection("persistanceMap") as PersistanceMapSection;
            if (section != null)
            {
                foreach (var element in section.Loggers)
                {
                    var type = Type.GetType(element.Type);
                    if (type != null)
                    {
                        var logger  = type.CreateInstance() as ILogger;
                        if (logger != null)
                        {
                            LoggerFactory.AddLogger(logger.GetType().Name, () => logger);
                        }
                    }
                }
            }
        }

        internal ILoggerFactory LoggerFactory { get; private set; }

        public void AddLogger(ILogger logger)
        {
            LoggerFactory.AddLogger(logger.GetType().Name, () => logger);
        }
    }
}
