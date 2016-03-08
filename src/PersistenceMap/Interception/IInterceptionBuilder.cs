using PersistenceMap.QueryBuilder;
using System;
using System.Collections.Generic;

namespace PersistenceMap.Interception
{
    public interface IInterceptionBuilder<T>
    {
        /// <summary>
        /// Adds a function that is executed before the Query is compiled. The function is applied to the Aggregate Type of a query.
        /// </summary>
        /// <param name="container">The IQueryPartsContainer containing all parts of the query</param>
        /// <returns>The interceptor</returns>
        IInterceptionBuilder<T> BeforeCompile(Action<IQueryPartsContainer> container);

        /// <summary>
        /// Adds a function that is executed before the Query is executed. The function is applied to the execution Type of the query
        /// </summary>
        /// <param name="query">The compiled query and the querystring</param>
        /// <returns>The interceptor</returns>
        IInterceptionBuilder<T> BeforeExecute(Action<CompiledQuery> query);

        /// <summary>
        /// Adds a function that is executed instead of executing against a RDBMS. The function is applied to the execution Type of the query. This function can be used to mock a call to the database.
        /// </summary>
        /// <param name="query">The compiled query and the querystring</param>
        /// <returns>The mocked data</returns>
        IInterceptionBuilder<T> AsExecute(Func<CompiledQuery, IEnumerable<T>> query);

        IInterceptionBuilder<T> AsExecute(Action<CompiledQuery> query);
    }
}
