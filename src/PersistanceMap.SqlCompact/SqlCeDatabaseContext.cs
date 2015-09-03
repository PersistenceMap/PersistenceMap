using PersistanceMap.Tracing;

namespace PersistanceMap
{
    public class SqlCeDatabaseContext : DatabaseContext, IDatabaseContext
    {
        public SqlCeDatabaseContext(IConnectionProvider provider, ILoggerFactory loggerFactory)
            : base(provider, loggerFactory)
        {
        }

        public SqlCeDatabaseContext(IConnectionProvider provider, ILoggerFactory loggerFactory, InterceptorCollection interceptors)
            : base(provider, loggerFactory, interceptors)
        {
        }
    }
}
