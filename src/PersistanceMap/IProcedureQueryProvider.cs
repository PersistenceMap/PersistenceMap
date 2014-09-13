using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface IProcedureQueryProvider : IQueryProvider
    {
        /// <summary>
        /// Adds a parameter containing the value of the expression
        /// </summary>
        /// <typeparam name="T2">The Type returned by the expression</typeparam>
        /// <param name="predicate">The Expression containing the value</param>
        /// <returns>IProcedureQueryProvider</returns>
        IProcedureQueryProvider AddParameter<T>(Expression<Func<T>> predicate);

        /// <summary>
        /// Adds a named parameter containing the value of the expression
        /// </summary>
        /// <typeparam name="T">The Type returned by the expression</typeparam>
        /// <param name="name">The name of the parameter</param>
        /// <param name="predicate">The Expression containing the value</param>
        /// <returns>IProcedureQueryProvider</returns>
        IProcedureQueryProvider AddParameter<T>(string name, Expression<Func<T>> predicate);

        /// <summary>
        /// Adds a named output parameter containing the value of the expression. The output is returned in the callback
        /// </summary>
        /// <typeparam name="T">The Type returned by the expression</typeparam>
        /// <param name="name">The name of the parameter</param>
        /// <param name="predicate">The Expression containing the value</param>
        /// <param name="callback">The callback for returning the output value</param>
        /// <returns>IProcedureQueryProvider</returns>
        IProcedureQueryProvider AddParameter<T>(string name, Expression<Func<T>> predicate, Action<T> callback);

        /// <summary>
        /// Creates a Type safe expression for the return value of the procedure call
        /// </summary>
        /// <typeparam name="T">The returned type</typeparam>
        /// <returns>A typesafe IProcedureQueryProvider</returns>
        IProcedureQueryProvider<T> For<T>();

        /// <summary>
        /// Creates a Type safe expression for the return value of the procedure call. The type is defined as a instance object passed as parameter.
        /// </summary>
        /// <typeparam name="T">The returned type</typeparam>
        /// <param name="anonymous">The instance defining the type. this can be a anonym object</param>
        /// <returns>A typesafe IProcedureQueryProvider</returns>
        IProcedureQueryProvider<T> For<T>(Expression<Func<T>> anonymous);

        /// <summary>
        /// Map a Property from the mapped type that is included in the result
        /// </summary>
        /// <typeparam name="T">The returned Type</typeparam>
        /// <typeparam name="TOut">The Property Type</typeparam>
        /// <param name="source">The name of the element in the resultset</param>
        /// <param name="alias">The Property to map to</param>
        /// <returns>IProcedureQueryProvider</returns>
        IProcedureQueryProvider Map<T, TOut>(string source, Expression<Func<T, TOut>> alias);

        /// <summary>
        /// Execute the Procedure without reading the resultset
        /// </summary>
        void Execute();

        /// <summary>
        /// Execute the Procedure
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> Execute<T>();
    }

    public interface IProcedureQueryProvider<T>
    {
        /// <summary>
        /// Map a Property from the mapped type that is included in the result
        /// </summary>
        /// <typeparam name="T">The returned Type</typeparam>
        /// <typeparam name="TOut">The Property Type</typeparam>
        /// <param name="source">The name of the element in the resultset</param>
        /// <param name="alias">The Property to map to</param>
        /// <returns>IProcedureQueryProvider</returns>
        IProcedureQueryProvider<T> Map<TOut>(string source, Expression<Func<T, TOut>> alias);

        /// <summary>
        /// Execute the Procedure
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> Execute();

        /// <summary>
        /// Execute the Procedure
        /// </summary>
        /// <returns></returns>
        IEnumerable<TOut> Execute<TOut>();
    }
}
