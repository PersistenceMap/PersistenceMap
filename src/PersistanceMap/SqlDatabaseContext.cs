using PersistanceMap.Tracing;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public class SqlDatabaseContext : DatabaseContext, IDatabaseContext
    {
        public SqlDatabaseContext(IConnectionProvider provider, ILoggerFactory loggerFactory)
            : base(provider, loggerFactory)
        {
        }

        #region Procedure Expressions

        public IProcedureQueryExpression Procedure(string procName)
        {
            return new ProcedureQueryProvider(this, procName);
        }

        #endregion
    }
}
