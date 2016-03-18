using System;
using PersistenceMap.QueryBuilder;
using System.Collections.Generic;

namespace PersistenceMap
{
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
        /// Executes the query against a RDBMS
        /// </summary>
        /// <param name="query">The query that will be executed</param>
        void Execute(CompiledQuery query);

        ///// <summary>
        ///// Executes a CompiledQuery that returnes multiple resultsets against the RDBMS
        ///// </summary>
        ///// <param name="compiledQuery">The CompiledQuery containing the expression</param>
        ///// <param name="expressions">All contexts that have to be parsed</param>
        //void Execute(CompiledQuery compiledQuery, params Action<IDataReaderContext>[] expressions);
    }
}
