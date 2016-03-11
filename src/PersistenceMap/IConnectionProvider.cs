using System;

namespace PersistenceMap
{
    /// <summary>
    /// The connection to a specific RDBMS
    /// </summary>
    public interface IConnectionProvider : IDisposable
    {
        /// <summary>
        /// The name of the database
        /// </summary>
        string Database { get; set; }

        /// <summary>
        /// The querycompiler that is needed to compiel a querypartscontainer to a sql statement
        /// </summary>
        IQueryCompiler QueryCompiler { get; }

        /// <summary>
        /// Execute the sql string to the RDBMS
        /// </summary>
        /// <param name="query">The query string</param>
        /// <returns>The contex containing the IDataReader</returns>
        IDataReaderContext Execute(string query);

        /// <summary>
        /// Execute the sql string to the RDBMS
        /// </summary>
        /// <param name="query">The query string</param>
        /// <returns>The amount of afected rows</returns>
        int ExecuteNonQuery(string query);
    }
}
