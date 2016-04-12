using System.Collections.Generic;

namespace PersistenceMap.Diagnostics
{
    public class LoggerConfiguration
    {
        private readonly List<ILogWriter> _writers;

        /// <summary>
        /// Creates an instance of a LoggerConfiguration that helps configure the LoggerFactory
        /// </summary>
        public LoggerConfiguration()
        {
            _writers = new List<ILogWriter>();
        }

        /// <summary>
        /// Add a LogWriter to the configuration
        /// </summary>
        /// <param name="writer">The LogWriter to add</param>
        /// <returns>The Configuration</returns>
        public LoggerConfiguration AddWriter(ILogWriter writer)
        {
            _writers.Add(writer);

            return this;
        }

        /// <summary>
        /// Adds all LogWriters to the default settings configuration. All LogWriters will be avaliable for all Queries
        /// </summary>
        /// <returns>The LoggerConfiguration</returns>
        public LoggerConfiguration SetDefault()
        {
            var configuration = Settings.Configuration();
            foreach (var writer in _writers)
            {
                configuration.AddWriter(writer);
            }

            return this;
        }

        /// <summary>
        /// Adds all LogWriters to the LoggerFactory of the Settings. LogWriters will only be avaliable for the instance of these settings
        /// </summary>
        /// <param name="settings">The PersistenceMap settings</param>
        /// <returns>The LoggerConfiguration</returns>
        public LoggerConfiguration SetToFactory(Settings settings)
        {
            foreach (var writer in _writers)
            {
                settings.AddLogWriter(writer);
            }

            return this;
        }
    }
}
