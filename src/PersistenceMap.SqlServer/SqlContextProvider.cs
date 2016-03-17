using PersistenceMap.Ensure;

namespace PersistenceMap
{
    /// <summary>
    /// Provides a context for Sql databases
    /// </summary>
    public class SqlContextProvider : ContextProvider, IContextProvider
    {
        /// <summary>
        /// Creates a provider to process operations against a SQL Database
        /// </summary>
        /// <param name="connectionProvider">The SQL Connection provider</param>
        public SqlContextProvider(IConnectionProvider connectionProvider)
            : base(connectionProvider)
        {
        }

        /// <summary>
        /// Creates a provider to process operations against a SQL Database
        /// </summary>
        /// <param name="connectionstring">The connectionstring for the SQL Database</param>
        public SqlContextProvider(string connectionstring)
            : base(new SqlConnectionProvider(connectionstring))
        {
            connectionstring.ArgumentNotNullOrEmpty("connectionstring");
        }

        /// <summary>
        /// Creates a context for connecting to a Sql Server database
        /// </summary>
        /// <returns>A DatabaseContext for SQL Databases</returns>
        public virtual SqlDatabaseContext Open()
        {
            return new SqlDatabaseContext(ConnectionProvider, Settings, Interceptors);
        }
    }
}
