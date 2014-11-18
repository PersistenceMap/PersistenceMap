using PersistanceMap.Tracing;

namespace PersistanceMap
{
    public class SqliteDatabaseContext : DatabaseContext, IDatabaseContext
    {
        public SqliteDatabaseContext(IConnectionProvider provider, ILoggerFactory loggerFactory)
            : base(provider, loggerFactory)
        {
        }

        public PersistanceMap.Sqlite.IDatabaseQueryExpression Database
        {
            get
            {
                return new PersistanceMap.Sqlite.QueryBuilder.DatabaseQueryBuilder(this);
            }
        }
    }
}
