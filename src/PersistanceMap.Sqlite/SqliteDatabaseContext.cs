using PersistanceMap.Diagnostics;
using PersistanceMap.Sqlite.QueryBuilder;

namespace PersistanceMap.Sqlite
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
