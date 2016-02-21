using PersistenceMap.Ensure;

namespace PersistenceMap
{
    public class SqlContextProvider : ContextProvider, IContextProvider
    {
        string _connectionString;

        public SqlContextProvider(string connectionstring)
            : base(new SqlConnectionProvider(connectionstring))
        {
            connectionstring.ArgumentNotNullOrEmpty("connectionstring");
            _connectionString = connectionstring;
        }

        /// <summary>
        /// Creates a context for connecting to a Sql Server database
        /// </summary>
        /// <returns>A DatabaseContext for SQL Databases</returns>
        public virtual SqlDatabaseContext Open()
        {
            //return new SqlDatabaseContext(ConnectionProvider, Settings, Interceptors); 
            return new SqlDatabaseContext(new SqlConnectionProvider(_connectionString), Settings, Interceptors);
        }
    }
}
