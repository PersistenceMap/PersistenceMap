namespace PersistanceMap
{
    /// <summary>
    /// Sql Context provider for SQL Server Compact
    /// </summary>
    public class SqlCeContextProvider : ContextProvider, IContextProvider
    {
        public SqlCeContextProvider(string connectionstring)
        {
            connectionstring.ArgumentNotNullOrEmpty("connectionstring");

            ConnectionProvider = new SqlCeConnectionProvider(connectionstring);
            Settings = new Settings();
        }
        
        /// <summary>
        /// Creates a context for connecting to a SqlCe database
        /// </summary>
        /// <returns></returns>
        public virtual SqlCeDatabaseContext Open()
        {
            return new SqlCeDatabaseContext(ConnectionProvider, Settings.LoggerFactory);
        }
    }
}
