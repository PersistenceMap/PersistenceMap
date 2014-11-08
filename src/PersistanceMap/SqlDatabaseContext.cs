using PersistanceMap.Diagnostics;

namespace PersistanceMap
{
    public class SqlDatabaseContext : DatabaseContext, IDatabaseContext
    {
        public SqlDatabaseContext(IConnectionProvider provider, ILoggerFactory loggerFactory)
            : base(provider, loggerFactory)
        {
        }
    }
}
