using System;
using PersistenceMap.QueryBuilder;
using System.Collections.Generic;

namespace PersistenceMap
{
    /// <summary>
    /// A context that can execute queries
    /// </summary>
    public interface IExecutionContext
    {
        /// <summary>
        /// Executes the query against a RDBMS
        /// </summary>
        /// <typeparam name="T">The expected return type</typeparam>
        /// <param name="query">The query that will be executed</param>
        /// <returns>A list of T</returns>
        IEnumerable<T> Execute<T>(CompiledQuery query);

        /// <summary>
        /// Executes the query against a RDBMS without retrieving a result
        /// </summary>
        /// <param name="query">The query that will be executed</param>
        void ExecuteNonQuery(CompiledQuery query);

        /// <summary>
        /// Executes the query against a RDBMS and parses all values to a Colleciton of ReaderResult
        /// </summary>
        /// <param name="query">The query to execute</param>
        /// <returns>All results as a List of ReaderResult</returns>
        IEnumerable<ReaderResult> Execute(CompiledQuery query);
    }
}
