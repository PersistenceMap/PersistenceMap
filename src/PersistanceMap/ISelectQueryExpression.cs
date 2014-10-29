using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface ISelectQueryExpressionBase<T> : IQueryExpression
    {
        IEnumerable<T2> Select<T2>();

        IEnumerable<T> Select();

        IEnumerable<TSelect> Select<TSelect>(Expression<Func<TSelect>> anonym);

        IEnumerable<TSelect> Select<TSelect>(Expression<Func<T, TSelect>> anonym);

        //T2 Single<T2>();

        IAfterMapQueryExpression<TNew> For<TNew>();

        IAfterMapQueryExpression<TAno> For<TAno>(Expression<Func<TAno>> anonym);

        /// <summary>
        /// Compiles the Query to a sql statement
        /// </summary>
        /// <typeparam name="T2">The select type</typeparam>
        /// <returns>The sql string</returns>
        string CompileQuery<T2>();

        /// <summary>
        /// Compiles the Query to a sql statement
        /// </summary>
        /// <returns>The sql string</returns>
        string CompileQuery();
    }

    public interface ISelectQueryExpression<T> : ISelectQueryExpressionBase<T>, IQueryExpression
    {
        /// <summary>
        /// Joines a new entity type to the last entity
        /// </summary>
        /// <typeparam name="TJoin">The type to join</typeparam>
        /// <param name="predicate">The expression that defines the connection</param>
        /// <param name="alias">The alias of the joining entity</param>
        /// <param name="source">The alias of the source entity</param>
        /// <returns>A IJoinQueryProvider{TJoin}</returns>
        IJoinQueryExpression<TJoin> Join<TJoin>(Expression<Func<TJoin, T, bool>> predicate, string alias = null, string source = null);

        /// <summary>
        /// Joines a new entity type to a previous entity
        /// </summary>
        /// <typeparam name="TJoin">The type to join</typeparam>
        /// <typeparam name="TOrig">The type of the previous entity to join to</typeparam>
        /// <param name="predicate">The expression that defines the connection</param>
        /// <param name="alias">The alias of the joining entity</param>
        /// <param name="source">The alias of the source entity</param>
        /// <returns>A IJoinQueryProvider{TJoin}</returns>
        IJoinQueryExpression<TJoin> Join<TJoin, TOrig>(Expression<Func<TJoin, TOrig, bool>> predicate, string alias = null, string source = null);

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type
        /// </summary>
        /// <param name="predicate">The expression that returns the Property</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryExpression<T> Map(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias defined (Table.Field as Alias)
        /// </summary>
        /// <param name="source">The expression that returns the Property</param>
        /// <param name="alias">The alias name the field will get (... as Alias)</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryExpression<T> Map(Expression<Func<T, object>> source, string alias);

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias from the select type
        /// </summary>
        /// <typeparam name="TAlias">The select type containig the alias property</typeparam>
        /// <param name="source">The source expression returning the source property</param>
        /// <param name="alias">The select expression returning the alias property</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryExpression<T> Map<TAlias>(Expression<Func<T, object>> source, Expression<Func<TAlias, object>> alias);

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias from the select type
        /// </summary>
        /// <typeparam name="TSource">The select type containig the source alias property</typeparam>
        /// <typeparam name="TAlias">The select type containig the alias property</typeparam>
        /// <param name="source">The source expression returning the source property</param>
        /// <param name="alias">The select expression returning the alias property</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryExpression<T> Map<TSource, TAlias>(Expression<Func<TSource, object>> source, Expression<Func<TAlias, object>> alias);

        /// <summary>
        /// Marks a field to be ignored in the query
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        ISelectQueryExpression<T> Ignore(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Marks a field to return the max value
        /// </summary>
        /// <param name="predicate">The expression that returns the Proerty to retrieve the value from</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryExpression<T> Max(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Marks a field to return the min value
        /// </summary>
        /// <param name="predicate">The expression that returns the Proerty to retrieve the value from<</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryExpression<T> Min(Expression<Func<T, object>> predicate);






        IWhereQueryExpression<T> Where(Expression<Func<T, bool>> predicate);

        IWhereQueryExpression<T> Where<T2>(Expression<Func<T2, bool>> predicate);

        IWhereQueryExpression<T> Where<T2, T3>(Expression<Func<T2, T3, bool>> predicate);




        IOrderQueryExpression<T> OrderBy(Expression<Func<T, object>> predicate);

        IOrderQueryExpression<T2> OrderBy<T2>(Expression<Func<T2, object>> predicate);

        IOrderQueryExpression<T> OrderByDesc(Expression<Func<T, object>> predicate);

        IOrderQueryExpression<T2> OrderByDesc<T2>(Expression<Func<T2, object>> predicate);

        
    }

    public interface IJoinQueryExpression<T> : ISelectQueryExpression<T>, IQueryExpression
    {
        IJoinQueryExpression<T> And<TAnd>(Expression<Func<T, TAnd, bool>> predicate, string alias = null, string source = null);

        IJoinQueryExpression<T> Or<TOr>(Expression<Func<T, TOr, bool>> predicate, string alias = null, string source = null);
    }

    public interface IWhereQueryExpression<T> : ISelectQueryExpressionBase<T>, IQueryExpression
    {
        IWhereQueryExpression<T> And(Expression<Func<T, bool>> predicate);

        IWhereQueryExpression<T> And<TAnd>(Expression<Func<TAnd, bool>> predicate, string alias = null);

        IWhereQueryExpression<T> And<TAnd>(Expression<Func<T, TAnd, bool>> predicate, string alias = null, string source = null);

        IWhereQueryExpression<T> And<TSource, TAnd>(Expression<Func<TSource, TAnd, bool>> predicate, string alias = null, string source = null);


        IWhereQueryExpression<T> Or(Expression<Func<T, bool>> predicate);

        IWhereQueryExpression<T> Or<TOr>(Expression<Func<TOr, bool>> predicate, string alias = null);

        IWhereQueryExpression<T> Or<TOr>(Expression<Func<T, TOr, bool>> predicate, string alias = null, string source = null);

        IWhereQueryExpression<T> Or<TSource, TOr>(Expression<Func<TSource, TOr, bool>> predicate, string alias = null, string source = null);



        IOrderQueryExpression<T> OrderBy(Expression<Func<T, object>> predicate);

        IOrderQueryExpression<T2> OrderBy<T2>(Expression<Func<T2, object>> predicate);

        IOrderQueryExpression<T> OrderByDesc(Expression<Func<T, object>> predicate);

        IOrderQueryExpression<T2> OrderByDesc<T2>(Expression<Func<T2, object>> predicate);
    }

    public interface IOrderQueryExpression<T> : ISelectQueryExpressionBase<T>, IQueryExpression
    {
        IOrderQueryExpression<T> ThenBy(Expression<Func<T, object>> predicate);

        IOrderQueryExpression<T> ThenBy<T2>(Expression<Func<T2, object>> predicate);

        IOrderQueryExpression<T> ThenByDesc(Expression<Func<T, object>> predicate);

        IOrderQueryExpression<T> ThenByDesc<T2>(Expression<Func<T2, object>> predicate);
    }

    public interface IAfterMapQueryExpression<T> : ISelectQueryExpressionBase<T>, IQueryExpression
    {
        //IAfterMapQueryExpression<T> AfterMap(Action<T> predicate);

        /// <summary>
        /// Marks the provided field as ignored. The field will not be included in the select.
        /// </summary>
        /// <param name="predicate">Marks the member to ignore</param>
        /// <returns>IAfterMapQueryProvider{T}</returns>
        IAfterMapQueryExpression<T> Ignore(Expression<Func<T, object>> predicate);
        
        /// <summary>
        /// Maps the provided field to a specific table. This helps to avoid Ambiguous column errors.
        /// </summary>
        /// <typeparam name="TSource">The source Table to map the member from</typeparam>
        /// <param name="predicate">Marks the member to be mapped</param>
        /// <returns>IAfterMapQueryProvider{T}</returns>
        IAfterMapQueryExpression<T> Map<TSource>(Expression<Func<TSource, object>> predicate);
    }
}
