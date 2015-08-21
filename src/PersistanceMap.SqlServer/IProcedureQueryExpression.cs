using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface IProcedureQueryExpression : IQueryExpression
    {
        /// <summary>
        /// Adds a parameter containing the value of the expression
        /// </summary>
        /// <typeparam name="T2">The Type returned by the expression</typeparam>
        /// <param name="predicate">The Expression containing the value</param>
        /// <returns>IProcedureQueryProvider</returns>
        IProcedureQueryExpression AddParameter<T>(Expression<Func<T>> predicate);

        /// <summary>
        /// Adds a named parameter containing the value of the expression
        /// </summary>
        /// <typeparam name="T">The Type returned by the expression</typeparam>
        /// <param name="name">The name of the parameter</param>
        /// <param name="predicate">The Expression containing the value</param>
        /// <returns>IProcedureQueryProvider</returns>
        IProcedureQueryExpression AddParameter<T>(string name, Expression<Func<T>> predicate);

        /// <summary>
        /// Adds a named output parameter containing the value of the expression. The output is returned in the callback
        /// </summary>
        /// <typeparam name="T">The Type returned by the expression</typeparam>
        /// <param name="name">The name of the parameter</param>
        /// <param name="predicate">The Expression containing the value</param>
        /// <param name="callback">The callback for returning the output value</param>
        /// <returns>IProcedureQueryProvider</returns>
        IProcedureQueryExpression AddParameter<T>(string name, Expression<Func<T>> predicate, Action<T> callback);

        /// <summary>
        /// Creates a Type safe expression for the return value of the procedure call
        /// </summary>
        /// <typeparam name="T">The returned type</typeparam>
        /// <returns>A typesafe IProcedureQueryProvider</returns>
        IProcedureQueryExpression<T> For<T>();

        /// <summary>
        /// Creates a Type safe expression for the return value of the procedure call. The type is defined as a instance object passed as parameter.
        /// </summary>
        /// <typeparam name="T">The returned type</typeparam>
        /// <param name="anonymous">The instance defining the type. this can be a anonym object</param>
        /// <returns>A typesafe IProcedureQueryProvider</returns>
        IProcedureQueryExpression<T> For<T>(Expression<Func<T>> anonymous);

        /// <summary>
        /// Map a Property from the mapped type that is included in the result
        /// </summary>
        /// <typeparam name="T">The returned Type</typeparam>
        /// <typeparam name="TOut">The Property Type</typeparam>
        /// <param name="source">The name of the element in the resultset</param>
        /// <param name="alias">The Property to map to</param>
        /// <param name="converter">The converter that converts the database value to the desired value in the dataobject</param>
        /// <returns>IProcedureQueryProvider</returns>
        IProcedureQueryExpression Map<T, TOut>(string source, Expression<Func<T, TOut>> alias, Expression<Func<object, object>> converter = null);

        /// <summary>
        /// Execute the Procedure without reading the resultset
        /// </summary>
        void Execute();

        /// <summary>
        /// Execute the Procedure and returns a list of T
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> Execute<T>();

        /// <summary>
        /// Execute the Procedure and returns a list of the type defined by the anonymous object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonymous"></param>
        /// <returns></returns>
        IEnumerable<T> Execute<T>(Expression<Func<T>> anonymous);
    }

    public interface IProcedureQueryExpression<T>
    {
        /// <summary>
        /// Map a Property from the mapped type that is included in the result
        /// </summary>
        /// <typeparam name="T">The returned Type</typeparam>
        /// <typeparam name="TOut">The Property Type</typeparam>
        /// <param name="source">The name of the element in the resultset</param>
        /// <param name="alias">The Property to map to</param>
        /// <param name="converter">The converter that converts the database value to the desired value in the dataobject</param>
        /// <returns>IProcedureQueryProvider</returns>
        IProcedureQueryExpression<T> Map<TOut>(string source, Expression<Func<T, TOut>> alias, Expression<Func<object, object>> converter = null);

        /// <summary>
        /// Ignore a Property from the mapped types
        /// </summary>
        /// <param name="member">The Property to ignore</param>
        /// <returns>IProcedureQueryProvider</returns>
        IProcedureQueryExpression<T> Ignore(Expression<Func<T, object>> member);

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
