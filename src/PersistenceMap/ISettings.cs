using PersistenceMap.Diagnostics;

namespace PersistenceMap
{
    public interface ISettings
    {
        ILoggerFactory LoggerFactory { get; }

        RestrictiveMode RestrictiveMappingMode { get; set; }

        void AddLogWriter(ILogWriter logger);
    }
}