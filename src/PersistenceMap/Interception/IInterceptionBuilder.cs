using PersistenceMap.QueryBuilder;
using System;

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
    }
}
