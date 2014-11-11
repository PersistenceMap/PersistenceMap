using System;

namespace PersistanceMap
{
    /// <summary>
    /// Sql Context provider for SQL Server Compact
    /// </summary>
    public class SqlCeContextProvider : IContextProvider
    {
        public SqlCeContextProvider(string connectionstring)
        {
            if (string.IsNullOrEmpty(connectionstring))
                throw new ArgumentNullException("connectionstring");

            ConnectionProvider = new SqlCeConnectionProvider(connectionstring);
            Settings = new Settings();
        }

        public Settings Settings { get; private set; }

        public IConnectionProvider ConnectionProvider { get; private set; }

        public virtual SqlCeDatabaseContext Open()
        {
            return new SqlCeDatabaseContext(ConnectionProvider, Settings.LoggerFactory);
        }

        #region IDisposeable Implementation

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        internal bool IsDisposed { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        public virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing && !IsDisposed)
                {
                    IsDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Releases resources before the object is reclaimed by garbage collection.
        /// </summary>
        ~SqlCeContextProvider()
        {
            Dispose(false);
        }

        #endregion
    }
}
