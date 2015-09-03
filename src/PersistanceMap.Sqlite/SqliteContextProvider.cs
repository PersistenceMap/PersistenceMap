using System;

namespace PersistanceMap
{
    public class SqliteContextProvider : ContextProvider, IContextProvider
    {
        public SqliteContextProvider(string connectionstring):base(new SqliteConnectionProvider(connectionstring))
        {
            if (string.IsNullOrEmpty(connectionstring))
            {
                throw new ArgumentNullException("connectionstring");
            }
        }

        /// <summary>
        /// Creates a context for connecting to a Sqlite database
        /// </summary>
        /// <returns>DatabaseContext for database operations</returns>
        public virtual SqliteDatabaseContext Open()
        {
            return new SqliteDatabaseContext(ConnectionProvider, Settings.LoggerFactory, Interceptors);
        }
    }
}
