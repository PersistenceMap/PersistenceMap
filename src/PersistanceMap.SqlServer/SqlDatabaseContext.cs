using PersistanceMap.Tracing;
using PersistanceMap.QueryBuilder;
using PersistanceMap.SqlServer;
using PersistanceMap.SqlServer.QueryBuilder;

namespace PersistanceMap
{
    public class SqlDatabaseContext : DatabaseContext, IDatabaseContext
    {
        public SqlDatabaseContext(IConnectionProvider provider, ILoggerFactory loggerFactory)
            : base(provider, loggerFactory)
        {
        }

        #region Database Expressions

        public IDatabaseQueryExpression Database
        {
            get
            {
                return new DatabaseQueryBuilder(this);
            }
        }

        #endregion

        #region Procedure Expressions

        public IProcedureQueryExpression Procedure(string procName)
        {
            return new ProcedureQueryProvider(this, procName);
        }

        #endregion
    }
}
