using PersistenceMap.Tracing;

namespace PersistenceMap
{
    public class SqliteDatabaseContext : DatabaseContext, IDatabaseContext
    {
        public SqliteDatabaseContext(IConnectionProvider provider, ISettings settings)
            : base(provider, settings)
        {
        }

        public SqliteDatabaseContext(IConnectionProvider provider, ISettings settings, InterceptorCollection interceptors)
            : base(provider, settings, interceptors)
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
