using System;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace PersistanceMap
{
    public class SqliteConnectionProvider : IConnectionProvider
    {
        public SqliteConnectionProvider(string connectionString)
        {
            ConnectionString = connectionString
                .Replace("Data Source =", "Data Source=")
                .Replace("data dource =", "Data Source=")
                .Replace("data source=", "Data Source=");
        }

        protected string ConnectionString { get; private set; }

        /// <summary>
        /// The name of the database
        /// </summary>
        public string Database
        {
            get
            {
                var regex = new Regex("Data Source=([^;]*);");
                var match = regex.Match(ConnectionString);
                if (match.Success)
                    return match.Value.Replace("Data Source=", "").Replace(";", "");

                return null;
            }
            set
            {
                // set new database name
                var regex = new Regex("Data Source=([^;]*);");
                ConnectionString = regex.Replace(ConnectionString, string.Format("Data Source={0};", value));
            }
        }

        private IQueryCompiler _queryCompiler;
        /// <summary>
        /// The querycompiler that is needed to compiel a querypartsmap to a sql statement
        /// </summary>
        public virtual IQueryCompiler QueryCompiler
        {
            get
            {
                if (_queryCompiler == null)
                    _queryCompiler = new QueryCompiler();

                return _queryCompiler;
            }
        }

        /// <summary>
        /// Execute the sql string to the RDBMS
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
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
        public void ExecuteNonQuery(string query)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
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
