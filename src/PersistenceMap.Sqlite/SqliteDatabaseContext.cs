using PersistenceMap.Tracing;

namespace PersistenceMap
{
    public class SqliteDatabaseContext : DatabaseContext, IDatabaseContext
    {
        public SqliteDatabaseContext(IConnectionProvider provider, ILoggerFactory loggerFactory)
            : base(provider, loggerFactory)
        {
        }

        public SqliteDatabaseContext(IConnectionProvider provider, ILoggerFactory loggerFactory, InterceptorCollection interceptors)
            : base(provider, loggerFactory, interceptors)
        {
        }

        public PersistenceMap.Sqlite.IDatabaseQueryExpression Database
        {
            get
            {
                return new PersistenceMap.Sqlite.QueryBuilder.DatabaseQueryBuilder(this);
            }
        }
    }
}
