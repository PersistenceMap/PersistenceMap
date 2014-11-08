using PersistanceMap.Diagnostics;

namespace PersistanceMap
{
    public class SqlCeDatabaseContext : DatabaseContext, IDatabaseContext
    {
        public SqlCeDatabaseContext(IConnectionProvider provider, ILoggerFactory loggerFactory)
            : base(provider, loggerFactory)
        {
        }
    }
}
