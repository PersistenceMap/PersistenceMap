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
                throw new ArgumentNullException("connectionString");

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
            SqlCeConnection connection;
            SqlCeCommand command;

            connection = new SqlCeConnection(ConnectionString);
            try
            {
                connection.Open();
                command = new SqlCeCommand(query, connection);
                
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
            SqlCeConnection connection;
            SqlCeCommand command;

            connection = new SqlCeConnection(ConnectionString);
            try
            {
                connection.Open();
                command = new SqlCeCommand(query, connection);
                
                command.ExecuteNonQuery();

                return new SqlCeContextReader(null, connection, command);

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                throw;
            }
        }
    }
}
