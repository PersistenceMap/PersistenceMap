using System;
using System.Data.SqlServerCe;
using System.Text.RegularExpressions;

namespace PersistanceMap
{
    public class SqlCeConnectionProvider : IConnectionProvider
    {
        public SqlCeConnectionProvider(string connectionString)
        {
            // format the string
            ConnectionString = connectionString.Replace("Data Source =", "Data Source=");
        }

        protected string ConnectionString { get; private set; }

        public string Database
        {
            get
            {
                var regex = new Regex("Data Source=([^;]*);");
                var match = regex.Match(ConnectionString);
                if (match.Success)
                    return match.Value;

                return null;
            }
            set
            {
                // set new database name
                var regex = new Regex("(?<=Data Source=).*(?=;)");
                ConnectionString = regex.Replace(ConnectionString, value);
            }
        }

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
            var connection = new SqlCeConnection(ConnectionString);

            connection.Open();
            var command = new SqlCeCommand(query, connection);

            return new SqlCeContextReader(command.ExecuteReader(), connection, command);
        }

        /// <summary>
        /// Executes a sql query without returning a resultset
        /// </summary>
        /// <param name="query"></param>
        public IReaderContext ExecuteNonQuery(string query)
        {
            var connection = new SqlCeConnection(ConnectionString);

            connection.Open();
            var command = new SqlCeCommand(query, connection);

            command.ExecuteNonQuery();

            return new SqlCeContextReader(null, connection, command);
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
        ~SqlCeConnectionProvider()
        {
            Dispose(false);
        }

        #endregion
    }
}
