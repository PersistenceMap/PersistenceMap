using PersistenceMap.Diagnostics;

namespace PersistenceMap
{
    public interface ISettings
    {
        ILogWriterFactory LoggerFactory { get; }

        RestrictiveMode RestrictiveMappingMode { get; set; }

        void AddLogger(ILogWriter logger);
    }
}