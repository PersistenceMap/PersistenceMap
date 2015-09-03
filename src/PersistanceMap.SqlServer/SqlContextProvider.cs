using System;

namespace PersistanceMap
{
    public class SqlContextProvider : ContextProvider, IContextProvider
    {
        public SqlContextProvider(string connectionstring)
            : base(new SqlConnectionProvider(connectionstring))
        {
            connectionstring.ArgumentNotNullOrEmpty(connectionstring);
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
