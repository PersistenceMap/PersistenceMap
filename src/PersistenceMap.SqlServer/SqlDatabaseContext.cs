using PersistenceMap.Tracing;
using PersistenceMap.QueryBuilder;
using PersistenceMap.SqlServer.QueryBuilder;

namespace PersistenceMap
{
    public class SqlDatabaseContext : DatabaseContext, IDatabaseContext
    {
        public SqlDatabaseContext(IConnectionProvider provider, ILoggerFactory loggerFactory, InterceptorCollection interceptors)
            : base(provider, loggerFactory, interceptors)
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

        /// <summary>
        /// Initializes a query expression for handling stored procedures
        /// </summary>
        /// <param name="procName">The name of the stored procedure</param>
        /// <returns>A IProcedureQueryExpression containing the information needed to execute a stored procedure</returns>
        public IProcedureQueryExpression Procedure(string procName)
        {
            return new ProcedureQueryProvider(this)
                .Procedure(procName);
        }

        #endregion
    }
}
