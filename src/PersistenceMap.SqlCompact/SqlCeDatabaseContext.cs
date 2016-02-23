using PersistenceMap.Diagnostics;

namespace PersistenceMap
{
    public class SqlCeDatabaseContext : DatabaseContext, IDatabaseContext
    {
        public SqlCeDatabaseContext(IConnectionProvider provider, ISettings settings)
            : base(provider, settings)
        {
        }

        public SqlCeDatabaseContext(IConnectionProvider provider, ISettings settings, InterceptorCollection interceptors)
            : base(provider, settings, interceptors)
        {
        }
    }
}
