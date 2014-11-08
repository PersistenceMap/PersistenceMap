using PersistanceMap.Diagnostics;

namespace PersistanceMap.Sqlite
{
    public class SqliteDatabaseContext : DatabaseContext, IDatabaseContext
    {
        public SqliteDatabaseContext(IConnectionProvider provider, ILoggerFactory loggerFactory)
            : base(provider, loggerFactory)
        {
        }
    }
}
