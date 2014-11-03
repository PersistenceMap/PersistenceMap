using PersistanceMap.Configuration;
using PersistanceMap.Diagnostics;
using PersistanceMap.Internals;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

namespace PersistanceMap
{
    public class Settings
    {
        public Settings()
        {
            _loggerFactory = new Lazy<ILoggerFactory>(() => 
            {
                // copy all loggers from the Configuration
                var factory = new LoggerFactory();
                ConfigSettings.Loggers.ForEach(l => factory.AddLogger(l.GetType().Name, l));
                return factory;
            });
        }

        
        static ConfigurationSettings _configurationSettings;
        private ConfigurationSettings ConfigSettings        
        {
            get
            {
                if (_configurationSettings == null)
                    _configurationSettings = new ConfigurationSettings();
                return _configurationSettings;
            }
        }

        readonly Lazy<ILoggerFactory> _loggerFactory;
        internal ILoggerFactory LoggerFactory
        {
            get
            {
                return _loggerFactory.Value;
            }
        }

        public void AddLogger(ILogger logger)
        {
            LoggerFactory.AddLogger(logger.GetType().Name, logger);
        }

        /// <summary>
        /// Class that is used to read the configuration from the app.config
        /// </summary>
        class ConfigurationSettings
        {
            public ConfigurationSettings()
            {
                var loggers = new List<ILogger>();

                // add loggers that are defined in the configurationsection in the app.config
                var section = ConfigurationManager.GetSection("persistanceMap") as PersistanceMapSection;
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
                            }
                            else
                            {
                                var loggerFactory = new LoggerFactory();
                                var logger = loggerFactory.CreateLogger();

                                var message = string.Format("Logger {0} cannot be created because the Type does not exist or does not derive from {1}.", element.Type, typeof(ILogger).Name);
                                
                                logger.Write(message, "Configuration error", "Configuration", DateTime.Now);
                                Trace.WriteLine(string.Format("#### PersistanceMap - Configuration error: {0}", message));
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
