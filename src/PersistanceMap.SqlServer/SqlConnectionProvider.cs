using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace PersistanceMap
{
    public class SqlConnectionProvider : IConnectionProvider
    {
        static SqlConnectionProvider()
        {
            // create a set of patterns how the catalog could be displayed in the connectionstring
            CatalogPatterns = new List<string>
            {
                "Initial Catalog =",
                "initial iatalog =",
                "initial iatalog=",
                "Database =",
                "Database=",
                "database =",
                "database="
            };
        }

        private static readonly IEnumerable<string> CatalogPatterns;

        public SqlConnectionProvider(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// The Connectionstring
        /// </summary>
        protected string ConnectionString { get; private set; }
        
        /// <summary>
        /// The name of the database
        /// </summary>
        public string Database
        {
            get
            {
                foreach (var pattern in CatalogPatterns)
                {
                    var regex = new Regex(string.Format("{0}([^;]*);", pattern));
                    var match = regex.Match(ConnectionString);
                    if (match.Success)
                    {
                        return match.Value.Replace(pattern, "").Replace(";", "");
                    }
                }

                return null;
            }
            set
            {
                // set new database name
                foreach (var pattern in CatalogPatterns)
                {
                    var regex = new Regex(string.Format("{0}([^;]*);", pattern));
                    var match = regex.Match(ConnectionString);
                    if (match.Success)
                    {
                        ConnectionString = regex.Replace(ConnectionString, string.Format("{0}{1};", pattern, value));
                        return;
                    }
                }
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
