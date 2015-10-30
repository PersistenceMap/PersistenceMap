using PersistenceMap.Ensure;

namespace PersistenceMap
{
    public class SqlContextProvider : ContextProvider, IContextProvider
    {
        public SqlContextProvider(string connectionstring)
            : base(new SqlConnectionProvider(connectionstring))
        {
            connectionstring.ArgumentNotNullOrEmpty("connectionstring");

            Settings = new Settings();
        }

        /// <summary>
        /// Creates a context for connecting to a Sql Server database
        /// </summary>
        /// <returns>A DatabaseContext for SQL Databases</returns>
        public virtual SqlDatabaseContext Open()
        {
            return new SqlDatabaseContext(ConnectionProvider, Settings.LoggerFactory, Interceptors);
        }
    }
}
