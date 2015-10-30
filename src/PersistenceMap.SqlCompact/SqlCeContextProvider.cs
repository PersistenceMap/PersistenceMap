using PersistenceMap.Ensure;

namespace PersistenceMap
{
    /// <summary>
    /// Sql Context provider for SQL Server Compact
    /// </summary>
    public class SqlCeContextProvider : ContextProvider, IContextProvider
    {
        public SqlCeContextProvider(string connectionstring)
            : base(new SqlCeConnectionProvider(connectionstring))
        {
            connectionstring.ArgumentNotNullOrEmpty("connectionstring");

            Settings = new Settings();
        }

        /// <summary>
        /// Creates a context for connecting to a SqlCe database
        /// </summary>
        /// <returns>DatabaseContext for database operations</returns>
        public virtual SqlCeDatabaseContext Open()
        {
            return new SqlCeDatabaseContext(ConnectionProvider, Settings.LoggerFactory, Interceptors);
        }
    }
}
