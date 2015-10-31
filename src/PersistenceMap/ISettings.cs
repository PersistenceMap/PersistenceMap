using PersistenceMap.Tracing;

namespace PersistenceMap
{
    public interface ISettings
    {
        ILoggerFactory LoggerFactory { get; }

        RestrictiveMode RestrictiveMappingMode { get; set; }

        void AddLogger(ILogger logger);
    }
}