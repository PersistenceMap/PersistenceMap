using System;

namespace PersistanceMap
{
    public class SqlContextProvider : IContextProvider
    {
        public SqlContextProvider(string connectionstring)
        {
            connectionstring.EnsureArgumentNotNullOrEmpty(connectionstring);

            ConnectionProvider = new SqlConnectionProvider(connectionstring);
            Settings = new Settings();
        }

        public Settings Settings { get; private set; }

        public IConnectionProvider ConnectionProvider { get; private set; }

        public virtual SqlDatabaseContext Open()
        {
            return new SqlDatabaseContext(ConnectionProvider, Settings.LoggerFactory);
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
        ~SqlContextProvider()
        {
            Dispose(false);
        }

        #endregion
    }
}
