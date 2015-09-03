using System;

namespace PersistanceMap
{
    /// <summary>
    /// Sql Context provider for SQL Server Compact
    /// </summary>
    public class SqlCeContextProvider : ContextProvider, IContextProvider
    {
        public SqlCeContextProvider(string connectionstring)
            : base(new SqlCeConnectionProvider(connectionstring))
        {
            if (string.IsNullOrEmpty(connectionstring))
            {
                throw new ArgumentNullException("connectionstring");
            }
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
