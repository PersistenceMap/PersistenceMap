using PersistanceMap.Compiler;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace PersistanceMap
{
    public class SqlContextProvider : IContextProvider
    {
        public SqlContextProvider(string connectionstring)
        {
            connectionstring.EnsureArgumentNotNullOrEmpty(connectionstring);

            ConnectionString = connectionstring;
        }

        public string ConnectionString { get; private set; }

        private IExpressionCompiler _expressionCompiler;
        public virtual IExpressionCompiler ExpressionCompiler
        {
            get
            {
                if (_expressionCompiler == null)
                    _expressionCompiler = new SqlExpressionCompiler();

                return _expressionCompiler;
            }
        }

        public virtual IReaderContext Execute(string query)
        {
            var connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                var command = new SqlCommand(query, connection);
                
                return new SqlContextReader(command.ExecuteReader(), connection, command);
            }
            catch (Exception ex)
            {
                Trace.TraceError("PersistanceMap - An error occured while executing a uery:\n{0}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Executes a sql query without returning a resultset
        /// </summary>
        /// <param name="query"></param>
        public IReaderContext ExecuteNonQuery(string query)
        {
            var connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                var command = new SqlCommand(query, connection);
                
                command.ExecuteNonQuery();

                return new SqlContextReader(null, connection, command);

            }
            catch (Exception ex)
            {
                Trace.TraceError("PersistanceMap - An error occured while executing a uery:\n{0}", ex.Message);
                throw;
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
        ~SqlContextProvider()
        {
            Dispose(false);
        }

        #endregion
    }
}
