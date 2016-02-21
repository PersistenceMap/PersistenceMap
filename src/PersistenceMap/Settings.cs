using PersistenceMap.Configuration;
using PersistenceMap.Tracing;
using PersistenceMap.Factories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

namespace PersistenceMap
{
    public class Settings : ISettings
    {
        private static ConfigurationSettings _configurationSettings;

        public Settings()
        {
            // initialize the loggerfactory
            _loggerFactory = new Lazy<ILoggerFactory>(() => 
            {
                // copy all loggers from the Configuration
                var factory = new LoggerFactory();
                ConfigSettings.Loggers.ForEach(l => factory.AddLogger(l.GetType().Name, l));
                return factory;
            });

            RestrictiveMappingMode = RestrictiveMode.Log;
        }

        /// <summary>
        /// Gets the singleton instace of the configuration settings that are defined in the *.config file
        /// </summary>
        private ConfigurationSettings ConfigSettings        
        {
            get
            {
                if (_configurationSettings == null)
                {
                    _configurationSettings = new ConfigurationSettings();
                }

                return _configurationSettings;
            }
        }

        readonly Lazy<ILoggerFactory> _loggerFactory;

        /// <summary>
        /// Gets the loggerfactory that containes all loggers that are defined in the configuraiton additionaly to the loggers added per instance of the settings
        /// </summary>
        public ILoggerFactory LoggerFactory
        {
            get
            {
                return _loggerFactory.Value;
            }
        }

        public RestrictiveMode RestrictiveMappingMode { get; set; }

        /// <summary>
        /// Adds a logger to the factory to the already defined loggers from the configuration
        /// </summary>
        /// <param name="logger">The logger to add to the loggerfactory</param>
        public void AddLogger(ILogger logger)
        {
            LoggerFactory.AddLogger(logger.GetType().Name, logger);
        }

        /// <summary>
        /// Class that is used to read the configuration from the app.config
        /// </summary>
        internal class ConfigurationSettings
        {
            public ConfigurationSettings()
            {
                var loggers = new List<ILogger>();

                // add loggers that are defined in the configurationsection in the app.config
                var section = ConfigurationManager.GetSection("PersistenceMap") as PersistenceMapSection;
                if (section != null)
                {
                    foreach (var element in section.Loggers)
                    {
                        var type = Type.GetType(element.Type);
                        if (type != null)
                        {
                            var instance = type.CreateInstance() as ILogger;
                            if (instance != null)
                            {
                                loggers.Add(instance);

                                Trace.WriteLine(string.Format("## PersistenceMap - Added Logger: {0} defined by the configuration", instance.GetType()));
                            }
                            else
                            {
                                var loggerFactory = new LoggerFactory();
                                var logger = loggerFactory.CreateLogger();

                                var message = string.Format("Logger {0} cannot be created because the Type does not exist or does not derive from {1}.", element.Type, typeof(ILogger).Name);
                                
                                logger.Write(message, "Configuration error", "Configuration", DateTime.Now);
                                Trace.WriteLine(string.Format("PersistenceMap - Configuration error: {0}", message));
                            }
                        }
                    }
                }

                Loggers = loggers;
            }

            internal IEnumerable<ILogger> Loggers { get; private set; }
        }
    }
}
