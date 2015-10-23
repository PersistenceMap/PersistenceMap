namespace PersistanceMap
{
    public class SqlContextProvider : ContextProvider, IContextProvider
    {
        public SqlContextProvider(string connectionstring)
        {
            connectionstring.ArgumentNotNullOrEmpty("connectionstring");

            ConnectionProvider = new SqlConnectionProvider(connectionstring);
            Settings = new Settings();
        }
        
        /// <summary>
        /// Creates a context for connecting to a Sql Server database
        /// </summary>
        /// <returns></returns>
        public virtual SqlDatabaseContext Open()
        {
            return new SqlDatabaseContext(ConnectionProvider, Settings.LoggerFactory);
        }
    }
}
