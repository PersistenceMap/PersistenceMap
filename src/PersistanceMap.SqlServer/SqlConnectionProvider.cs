using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

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
        /// The querycompiler that is needed to compiel a querypartscontainer to a sql statement
        /// </summary>
        public virtual IQueryCompiler QueryCompiler
        {
            get
            {
                if (_queryCompiler == null)
                    _queryCompiler = new SqlServer.QueryCompiler();

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
                var executer = connection.GetExecuter(query);
                executer.ExecuteNonQuery(connection, query);
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

    internal static class SqlConnectionExtensions
    {
        public static IQueryExecuter GetExecuter(this SqlConnection connection, string query)
        {
            var regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (regex.Match(query).Success)
            {
                return new TransactionedQueryExeuter();
            }

            return new QueryExecuter();
        }
    }

    internal interface IQueryExecuter
    {
        void ExecuteNonQuery(SqlConnection connection, string query);
    }

    internal class QueryExecuter : IQueryExecuter
    {
        public void ExecuteNonQuery(SqlConnection connection, string query)
        {
            using (var command = new SqlCommand(query, connection))
            {
                try
                {
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    System.Diagnostics.Trace.WriteLine(e);
                    throw e;
                }
            }
        }
    }

    internal class TransactionedQueryExeuter : IQueryExecuter
    {
        public void ExecuteNonQuery(SqlConnection connection, string query)
        {
            // SqlCommand can't handle go breakes so split all go
            var regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string[] lines = regex.Split(query);

            var transaction = connection.BeginTransaction();
            
            using (var command = connection.CreateCommand())
            {
                try
                {
                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            command.CommandText = line;
                            command.Transaction = transaction;

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (SqlException e)
                {
                    transaction.Rollback();
                    System.Diagnostics.Trace.WriteLine(e);
                    throw e;
                }
            }

            transaction.Commit();
        }
    }
}
