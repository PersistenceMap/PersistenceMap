using PersistanceMap.Ensure;

namespace PersistanceMap
{
    public class SqliteContextProvider : ContextProvider, IContextProvider
    {
        public SqliteContextProvider(string connectionstring)
        {
            connectionstring.ArgumentNotNullOrEmpty("connectionstring");

            ConnectionProvider = new SqliteConnectionProvider(connectionstring);
            Settings = new Settings();
        }
        
        /// <summary>
        /// Creates a context for connecting to a Sqlite database
        /// </summary>
        /// <returns></returns>
        public virtual SqliteDatabaseContext Open()
        {
            return new SqliteDatabaseContext(ConnectionProvider, Settings.LoggerFactory);
        }
    }
}
