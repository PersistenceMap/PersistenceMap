using PersistanceMap.Sqlite;
using PersistanceMap.Sqlite.QueryBuilder;
using PersistanceMap.Tracing;

namespace PersistanceMap
{
    public class SqliteDatabaseContext : DatabaseContext, IDatabaseContext
    {
        public SqliteDatabaseContext(IConnectionProvider provider, ILoggerFactory loggerFactory)
            : base(provider, loggerFactory)
        {
        }

        public IDatabaseQueryExpression Database
        {
            get
            {
                return new DatabaseQueryBuilder(this);
            }
        }
    }
}
