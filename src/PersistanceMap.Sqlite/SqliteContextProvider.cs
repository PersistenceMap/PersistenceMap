using System;

namespace PersistanceMap
{
    public class SqliteContextProvider : IContextProvider
    {
        public SqliteContextProvider(string connectionstring)
        {
            if (string.IsNullOrEmpty(connectionstring))
                throw new ArgumentNullException("connectionstring");

            ConnectionProvider = new SqliteConnectionProvider(connectionstring);
            Settings = new Settings();
        }

        public Settings Settings { get; private set; }

        /// <summary>
        /// The connection to a Sqlite database
        /// </summary>
        public IConnectionProvider ConnectionProvider { get; private set; }

        /// <summary>
        /// Creates a context for connecting to a Sqlite database
        /// </summary>
        /// <returns></returns>
        public virtual SqliteDatabaseContext Open()
        {
            return new SqliteDatabaseContext(ConnectionProvider, Settings.LoggerFactory);
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
        ~SqliteContextProvider()
        {
            Dispose(false);
        }

        #endregion
    }
}
