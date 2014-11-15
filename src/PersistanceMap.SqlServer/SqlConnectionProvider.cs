using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace PersistanceMap
{
    public class SqlConnectionProvider : IConnectionProvider
    {
        public SqlConnectionProvider(string connectionString)
        {
            ConnectionString = connectionString
                .Replace("Initial Catalog =", "Initial Catalog=")
                .Replace("initial iatalog =", "Initial Catalog=")
                .Replace("initial iatalog=", "Initial Catalog=")
                .Replace("Database =", "Initial Catalog=")
                .Replace("Database=", "Initial Catalog=")
                .Replace("database =", "Initial Catalog=")
                .Replace("database=", "Initial Catalog=");
        }

        protected string ConnectionString { get; private set; }

        public string Database
        {
            get
            {
                var regex = new Regex("Initial Catalog=([^;]*);");
                var match = regex.Match(ConnectionString);
                if (match.Success)
                    return match.Value.Replace("Initial Catalog=", "").Replace(";", "");

                return null;
            }
            set
            {
                // set new database name
                var regex = new Regex("Initial Catalog=([^;]*);");
                ConnectionString = regex.Replace(ConnectionString, string.Format("Initial Catalog={0};", value));
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
            var connection = new SqlConnection(ConnectionString);

            connection.Open();
            var command = new SqlCommand(query, connection);

            return new SqlContextReader(command.ExecuteReader(), connection, command);
        }

        /// <summary>
        /// Executes a sql query without returning a resultset
        /// </summary>
        /// <param name="query"></param>
        public IReaderContext ExecuteNonQuery(string query)
        {
            var connection = new SqlConnection(ConnectionString);

            connection.Open();
            var command = new SqlCommand(query, connection);

            command.ExecuteNonQuery();

            return new SqlContextReader(null, connection, command);
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
        ~SqlConnectionProvider()
        {
            Dispose(false);
        }

        #endregion
    }
}
