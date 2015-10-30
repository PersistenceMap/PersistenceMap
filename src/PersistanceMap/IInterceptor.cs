using System;
using System.Collections.Generic;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    /// <summary>
    /// Defines an interceptor
    /// </summary>
    public interface IInterceptor
    {
    }

    /// <summary>
    /// Defines an interceptor
    /// </summary>
    /// <typeparam name="T">Type to intercept</typeparam>
    public interface IInterceptor<T> : IInterceptor
    {
        /// <summary>
        /// Adds a function that is executed before the Query is compiled. The function is applied to the Aggregate Type of a query.
        /// </summary>
        /// <param name="container">The IQueryPartsContainer containing all parts of the query</param>
        /// <returns>The interceptor</returns>
        IInterceptor<T> BeforeCompile(Action<IQueryPartsContainer> container);

        /// <summary>
        /// Adds a function that is executed before the Query is executed. The function is applied to the execution Type of the query
        /// </summary>
        /// <param name="query">The compiled query and the querystring</param>
        /// <returns>The interceptor</returns>
        IInterceptor<T> BeforeExecute(Action<CompiledQuery> query);

        /// <summary>
        /// Adds a function that is executed instead of executing against a RDBMS. The function is applied to the execution Type of the query. This function can be used to mock a call to the database.
        /// </summary>
        /// <param name="query">The compiled query and the querystring</param>
        /// <returns>The mocked data</returns>
        IInterceptor<T> AsExecute(Func<CompiledQuery, IEnumerable<T>> query);

        IInterceptor<T> AsExecute(Action<CompiledQuery> query);
    }

    /// <summary>
    /// The execution part of the interceptor
    /// </summary>
    public interface IInterceptorExecution : IInterceptor
    {
        void ExecuteBeforeExecute(CompiledQuery query);

        IEnumerable<T> Execute<T>(CompiledQuery query);

        bool Execute(CompiledQuery query);

        void ExecuteBeforeCompile(IQueryPartsContainer container);
    }
}
