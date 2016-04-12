using PersistenceMap.Configuration;
using PersistenceMap.Diagnostics;
using System;

namespace PersistenceMap
{
    public class Settings : ISettings
    {
        private static SettingsConfiguration _configurationSettings;
        private Lazy<ILoggerFactory> _loggerFactory;

        public Settings()
        {
            // initialize the loggerfactory
            Initialize();

            RestrictiveMappingMode = RestrictiveMode.Log;
        }

        /// <summary>
        /// Gets the singleton instace of the configuration settings that are defined in the *.config file
        /// </summary>
        internal static SettingsConfiguration Configuration()
        {
            if (_configurationSettings == null)
            {
                _configurationSettings = new SettingsConfiguration();
            }

            return _configurationSettings;
        }

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
        /// <param name="writer">The logger to add to the loggerfactory</param>
        public void AddLogWriter(ILogWriter writer)
        {
            LoggerFactory.AddWriter(writer.GetType().Name, writer);
        }

        /// <summary>
        /// Reset all settings for all instances to default
        /// </summary>
        public void Reset()
        {
            _configurationSettings = null;
            Initialize();
        }

        private void Initialize()
        {
            // initialize the loggerfactory
            _loggerFactory = new Lazy<ILoggerFactory>(() =>
            {
                // copy all loggers from the Configuration
                var factory = new LoggerFactory();
                Configuration().Loggers.ForEach(l => factory.AddWriter(l.GetType().Name, l));
                return factory;
            });
        }
    }
}
