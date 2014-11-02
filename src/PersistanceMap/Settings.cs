using PersistanceMap.Configuration;
using PersistanceMap.Diagnostics;
using PersistanceMap.Internals;
using System;
using System.Configuration;
using System.Diagnostics;

namespace PersistanceMap
{
    public class Settings
    {
        public Settings()
        {
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

        internal ILoggerFactory LoggerFactory
        {
            get
            {
                return ConfigSettings.LoggerFactory;
            }
        }

        public void AddLogger(ILogger logger)
        {
            LoggerFactory.AddLogger(logger.GetType().Name, () => logger);
        }

        /// <summary>
        /// Class that is used to read the configuration from the app.config
        /// </summary>
        class ConfigurationSettings
        {
            public ConfigurationSettings()
            {
                LoggerFactory = new LoggerFactory();

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
                                LoggerFactory.AddLogger(instance.GetType().Name, () => instance);
                            }
                            else
                            {
                                var message = string.Format("#### PersistanceMap - Configuration error: Logger {0} cannot be created because the Type does not exist or does not derive from {1}.", element.Type, typeof(ILogger).Name);
                                var logger = LoggerFactory.CreateLogger();
                                logger.Write(message, "Configuration", DateTime.Now);
                                Trace.WriteLine(message);
                            }
                        }
                    }
                }
            }

            internal ILoggerFactory LoggerFactory { get; private set; }
        }
    }
}
