using System;
using System.Data.SqlClient;

namespace PersistanceMap
{
    public class SqlConnectionProvider : IConnectionProvider
    {
        private readonly Lazy<ConnectionStringBuilder> _connectionStringBuilder;

        public SqlConnectionProvider(string connectionString)
        {
            ConnectionString = connectionString;
            _connectionStringBuilder = new Lazy<ConnectionStringBuilder>(() => new ConnectionStringBuilder());
        }

        /// <summary>
        /// The connectionstring
        /// </summary>
        protected string ConnectionString { get; private set; }

        /// <summary>
        /// The name of the database
        /// </summary>
        public string Database
        {
            get
            {
                return _connectionStringBuilder.Value.GetDatabase(ConnectionString);
            }
            set
            {
                // set new database name
                ConnectionString = _connectionStringBuilder.Value.SetDatabase(value, ConnectionString);
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
            var connection = new SqlConnection(ConnectionString);

            connection.Open();
            var command = new SqlCommand(query, connection);

            return new SqlContextReader(command.ExecuteReader(), connection, command);
        }

        /// <summary>
        /// Executes a sql query without returning a resultset
        /// </summary>
        /// <param name="query"></param>
        public void ExecuteNonQuery(string query)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
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
        ~SqlConnectionProvider()
        {
            Dispose(false);
        }

        #endregion
    }
}
