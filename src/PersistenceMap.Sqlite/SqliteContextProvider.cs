using PersistenceMap.Ensure;

namespace PersistenceMap
{
    /// <summary>
    /// Provides a context for Sqlite databases
    /// </summary>
    public class SqliteContextProvider : ContextProvider, IContextProvider
    {
        /// <summary>
        /// Creates a provider to process operations against a Sqlite Database
        /// </summary>
        /// <param name="connectionProvider">The Sqlite Connection provider</param>
        public SqliteContextProvider(IConnectionProvider connectionProvider)
            : base(connectionProvider)
        {
        }

        /// <summary>
        /// Creates a provider to process operations against a Sqlite Database
        /// </summary>
        /// <param name="connectionstring">The connectionstring for the Sqlite Database</param>
        public SqliteContextProvider(string connectionstring)
            : base(new SqliteConnectionProvider(connectionstring))
        {
            connectionstring.ArgumentNotNullOrEmpty("connectionstring");
        }

        /// <summary>
        /// Creates a context for connecting to a Sqlite database
        /// </summary>
        /// <returns>DatabaseContext for database operations</returns>
        public virtual SqliteDatabaseContext Open()
        {
            return new SqliteDatabaseContext(ConnectionProvider, Settings, Interceptors);
        }
    }
}
