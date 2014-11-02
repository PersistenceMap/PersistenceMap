using PersistanceMap.Configuration;
using PersistanceMap.Diagnostics;
using PersistanceMap.Internals;
using System;
using System.Configuration;

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
                            var logger = type.CreateInstance() as ILogger;
                            if (logger != null)
                            {
                                LoggerFactory.AddLogger(logger.GetType().Name, () => logger);
                            }
                        }
                    }
                }
            }

            internal ILoggerFactory LoggerFactory { get; private set; }
        }
    }
}
