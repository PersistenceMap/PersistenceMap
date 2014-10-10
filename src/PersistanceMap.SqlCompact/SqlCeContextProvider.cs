using PersistanceMap.Compiler;
using System;
using System.Data.SqlServerCe;
using System.Diagnostics;

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

            ConnectionString = connectionstring;
        }

        public string ConnectionString { get; private set; }

        private IExpressionCompiler _expressionCompiler;
        public virtual IExpressionCompiler ExpressionCompiler
        {
            get
            {
                if (_expressionCompiler == null)
                    _expressionCompiler = new ExpressionCompiler();

                return _expressionCompiler;
            }
        }

        public virtual IReaderContext Execute(string query)
        {
            var connection = new SqlCeConnection(ConnectionString);
            try
            {
                connection.Open();
                var command = new SqlCeCommand(query, connection);
                
                return new SqlCeContextReader(command.ExecuteReader(), connection, command);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// Executes a sql query without returning a resultset
        /// </summary>
        /// <param name="query"></param>
        public IReaderContext ExecuteNonQuery(string query)
        {
            var connection = new SqlCeConnection(ConnectionString);
            try
            {
                connection.Open();
                var command = new SqlCeCommand(query, connection);
                
                command.ExecuteNonQuery();

                return new SqlCeContextReader(null, connection, command);

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
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
        ~SqlCeContextProvider()
        {
            Dispose(false);
        }

        #endregion
    }
}
