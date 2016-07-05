using PersistenceMap.Diagnostics;

namespace PersistenceMap
{
    public interface ISettings
    {
        /// <summary>
        /// Gets the loggerfactory that containes all loggers that are defined in the configuraiton additionaly to the loggers added per instance of the settings
        /// </summary>
        ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Gets or sets how restrictive the mapper handles errors
        /// </summary>
        RestrictiveMode RestrictiveMappingMode { get; set; }

        /// <summary>
        /// Gets or sets the depth of information that is logged
        /// </summary>
        LogDebth LogLevel { get; set; }

        /// <summary>
        /// Adds a logwriter to the factory
        /// </summary>
        /// <param name="logger">The logwriter</param>
        void AddLogWriter(ILogWriter logger);
    }
}