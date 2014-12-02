using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface ISelectQueryExpressionBase<T> : IQueryExpression
    {
        /// <summary>
        /// Executes a select expression and maps the returnvalue to objects of the defined type
        /// </summary>
        /// <typeparam name="T2">The type to return</typeparam>
        /// <returns></returns>
        IEnumerable<T2> Select<T2>();

        /// <summary>
        /// Executes a select expression and maps the returnvalue to objects of the defined type
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> Select();

        /// <summary>
        /// Executes a select expression and maps the returnvalue to objects of the defined type
        /// </summary>
        /// <typeparam name="TSelect">The type to return</typeparam>
        /// <param name="anonym">The type to return</param>
        /// <returns></returns>
        IEnumerable<TSelect> Select<TSelect>(Expression<Func<TSelect>> anonym);

        /// <summary>
        /// Executes a select expression and maps the returnvalue to objects of the defined type and executes all objects to the delegate
        /// </summary>
        /// <typeparam name="TSelect">The type to return</typeparam>
        /// <param name="anonym">The delegate that gets executed for each returned object</param>
        /// <returns></returns>
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
        /// Map a Property that is included in the result that belongs to a joined type with an alias defined (Table.Field as Alias)
        /// </summary>
        /// <param name="source">The expression that returns the Property</param>
        /// <param name="alias">The alias name the field will get (... as Alias)</param>
        /// <param name="converter">The converter that converts the database value to the desired value in the dataobject</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryExpression<T> Map<TProp>(Expression<Func<T, TProp>> source, string alias = null, Expression<Func<TProp, object>> converter = null);
        
        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias from the select type
        /// </summary>
        /// <typeparam name="TAlias">The select type containig the alias property</typeparam>
        /// <param name="source">The source expression returning the source property</param>
        /// <param name="alias">The select expression returning the alias property</param>
        /// <param name="converter">The converter that converts the database value to the desired value in the dataobject</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryExpression<T> Map<TAlias>(Expression<Func<T, object>> source, Expression<Func<TAlias, object>> alias, Expression<Func<object, object>> converter = null);

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias from the select type
        /// </summary>
        /// <typeparam name="TSource">The select type containig the source alias property</typeparam>
        /// <typeparam name="TAlias">The select type containig the alias property</typeparam>
        /// <param name="source">The source expression returning the source property</param>
        /// <param name="alias">The select expression returning the alias property</param>
        /// <param name="converter">The converter that converts the database value to the desired value in the dataobject</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryExpression<T> Map<TSource, TAlias>(Expression<Func<TSource, object>> source, Expression<Func<TAlias, object>> alias, Expression<Func<object, object>> converter = null);

        /// <summary>
        /// Marks a field to be ignored in the query
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        ISelectQueryExpression<T> Ignore(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Marks a field to return the max value of
        /// </summary>
        /// <param name="predicate">The expression that returns the Proerty to retrieve the value from</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryExpression<T> Max(Expression<Func<T, object>> predicate, string alias = null);

        /// <summary>
        /// Marks a field to return the min value of
        /// </summary>
        /// <param name="predicate">The expression that returns the Proerty to retrieve the value from<</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryExpression<T> Min(Expression<Func<T, object>> predicate, string alias = null);

        /// <summary>
        /// Marks a field to return the count of
        /// </summary>
        /// <param name="predicate">The expression that returns the Proerty to retrieve the value from<</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryExpression<T> Count(Expression<Func<T, object>> predicate, string alias = null);




        IWhereQueryExpression<T> Where(Expression<Func<T, bool>> operation);

        IWhereQueryExpression<T> Where<T2>(Expression<Func<T2, bool>> operation);

        IWhereQueryExpression<T> Where<T2, T3>(Expression<Func<T2, T3, bool>> operation);



        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> OrderBy(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T2> OrderBy<T2>(Expression<Func<T2, object>> predicate);

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> OrderByDesc(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T2> OrderByDesc<T2>(Expression<Func<T2, object>> predicate);

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        IGroupQueryExpression<T> GroupBy(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <typeparam name="T2">The type containing the member to group by</typeparam>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        IGroupQueryExpression<T> GroupBy<T2>(Expression<Func<T2, object>> predicate);
    }

    public interface IJoinQueryExpression<T> : ISelectQueryExpression<T>, IQueryExpression
    {
        IJoinQueryExpression<T> And<TAnd>(Expression<Func<T, TAnd, bool>> operation, string alias = null, string source = null);

        IJoinQueryExpression<T> Or<TOr>(Expression<Func<T, TOr, bool>> operation, string alias = null, string source = null);
    }

    public interface IWhereQueryExpression<T> : ISelectQueryExpressionBase<T>, IQueryExpression
    {
        IWhereQueryExpression<T> And(Expression<Func<T, bool>> operation);

        IWhereQueryExpression<T> And<TAnd>(Expression<Func<TAnd, bool>> operation, string alias = null);

        IWhereQueryExpression<T> And<TAnd>(Expression<Func<T, TAnd, bool>> operation, string alias = null, string source = null);

        IWhereQueryExpression<T> And<TSource, TAnd>(Expression<Func<TSource, TAnd, bool>> operation, string alias = null, string source = null);


        IWhereQueryExpression<T> Or(Expression<Func<T, bool>> operation);

        IWhereQueryExpression<T> Or<TOr>(Expression<Func<TOr, bool>> operation, string alias = null);

        IWhereQueryExpression<T> Or<TOr>(Expression<Func<T, TOr, bool>> operation, string alias = null, string source = null);

        IWhereQueryExpression<T> Or<TSource, TOr>(Expression<Func<TSource, TOr, bool>> operation, string alias = null, string source = null);



        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        IGroupQueryExpression<T> GroupBy(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <typeparam name="T2">The type containing the member to group by</typeparam>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        IGroupQueryExpression<T> GroupBy<T2>(Expression<Func<T2, object>> predicate);

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> OrderBy(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T2> OrderBy<T2>(Expression<Func<T2, object>> predicate);

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> OrderByDesc(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T2> OrderByDesc<T2>(Expression<Func<T2, object>> predicate);
    }

    public interface IOrderQueryExpression<T> : ISelectQueryExpressionBase<T>, IQueryExpression
    {
        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> ThenBy(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> ThenBy<T2>(Expression<Func<T2, object>> predicate);

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> ThenByDesc(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> ThenByDesc<T2>(Expression<Func<T2, object>> predicate);
    }

    public interface IGroupQueryExpression<T> : ISelectQueryExpressionBase<T>, IQueryExpression
    {
        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        IGroupQueryExpression<T> ThenBy(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <typeparam name="T2">The type containing the member to group by</typeparam>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        IGroupQueryExpression<T> ThenBy<T2>(Expression<Func<T2, object>> predicate);

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> OrderBy(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T2> OrderBy<T2>(Expression<Func<T2, object>> predicate);

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> OrderByDesc(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T2> OrderByDesc<T2>(Expression<Func<T2, object>> predicate);
    }

    public interface IAfterMapQueryExpression<T> : ISelectQueryExpressionBase<T>, IQueryExpression
    {
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

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        IGroupQueryExpression<T> GroupBy(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <typeparam name="T2">The type containing the member to group by</typeparam>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        IGroupQueryExpression<T> GroupBy<T2>(Expression<Func<T2, object>> predicate);

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> OrderBy(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T2> OrderBy<T2>(Expression<Func<T2, object>> predicate);
    }
}
