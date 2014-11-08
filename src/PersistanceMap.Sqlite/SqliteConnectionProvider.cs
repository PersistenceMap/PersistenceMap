using System;
using System.Data.SQLite;

namespace PersistanceMap.Sqlite
{
    public class SqliteConnectionProvider : IConnectionProvider
    {
        public SqliteConnectionProvider(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; private set; }

        private IQueryCompiler _queryCompiler;
        public virtual IQueryCompiler QueryCompiler
        {
            get
            {
                if (_queryCompiler == null)
                    _queryCompiler = new QueryCompiler();

                return _queryCompiler;
            }
        }

        public virtual IReaderContext Execute(string query)
        {
            var connection = new SQLiteConnection(ConnectionString);

            connection.Open();
            var command = new SQLiteCommand(query, connection);

            return new SqliteContextReader(command.ExecuteReader(), connection, command);
        }

        /// <summary>
        /// Executes a sql query without returning a resultset
        /// </summary>
        /// <param name="query"></param>
        public IReaderContext ExecuteNonQuery(string query)
        {
            var connection = new SQLiteConnection(ConnectionString);

            connection.Open();
            var command = new SQLiteCommand(query, connection);

            command.ExecuteNonQuery();

            return new SqliteContextReader(null, connection, command);
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
        ~SqliteConnectionProvider()
        {
            Dispose(false);
        }

        #endregion
    }
}
