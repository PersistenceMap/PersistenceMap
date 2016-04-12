using PersistenceMap.Diagnostics;
using PersistenceMap.Factories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

namespace PersistenceMap.Configuration
{
    /// <summary>
    /// Class that is used to read the configuration from the app.config
    /// </summary>
    internal class SettingsConfiguration
    {
        private readonly IList<ILogWriter> _writers;

        public SettingsConfiguration()
        {
            var loggers = new List<ILogWriter>();

            // add loggers that are defined in the configurationsection in the app.config
            var section = ConfigurationManager.GetSection("PersistenceMap") as PersistenceMapSection;
            if (section != null)
            {
                foreach (var element in section.Loggers)
                {
                    var type = Type.GetType(element.Type);
                    if (type != null)
                    {
                        var instance = type.CreateInstance() as ILogWriter;
                        if (instance != null)
                        {
                            loggers.Add(instance);

                            Trace.WriteLine(string.Format("## PersistenceMap - Added Logger: {0} defined by the configuration", instance.GetType()));
                        }
                        else
                        {
                            var loggerFactory = new LoggerFactory();
                            var logger = loggerFactory.CreateLogger();

                            var message = string.Format("Logger {0} cannot be created because the Type does not exist or does not derive from {1}.", element.Type, typeof(ILogWriter).Name);

                            logger.Write(message, "Configuration error", "Configuration", DateTime.Now);
                            Trace.WriteLine(string.Format("PersistenceMap - Configuration error: {0}", message));
                        }
                    }
                }
            }

            _writers = loggers;
        }

        internal IEnumerable<ILogWriter> Loggers => _writers;

        internal SettingsConfiguration AddWriter(ILogWriter writer)
        {
            _writers.Add(writer);

            return this;
        }
    }
}
