using PersistanceMap.Tracing;

namespace PersistanceMap
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

        public PersistanceMap.Sqlite.IDatabaseQueryExpression Database
        {
            get
            {
                return new PersistanceMap.Sqlite.QueryBuilder.DatabaseQueryBuilder(this);
            }
        }
    }
}
