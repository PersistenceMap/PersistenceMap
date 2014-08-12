using PersistanceMap.QueryProvider;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public interface ISelectQueryProviderBase<T> : IQueryProvider
    {
        IEnumerable<T2> Select<T2>();

        IEnumerable<T> Select();

        T2 Single<T2>();
        
        /// <summary>
        /// Compiles the Query to a sql statement
        /// </summary>
        /// <typeparam name="T">The select type</typeparam>
        /// <returns>The sql string</returns>
        string CompileQuery<T2>();
    }

    public interface ISelectQueryProvider<T> : ISelectQueryProviderBase<T>, IQueryProvider
    {
        /// <summary>
        /// Joines a new entity type to the last entity
        /// </summary>
        /// <typeparam name="TJoin">The type to join</typeparam>
        /// <param name="predicate">The expression that defines the connection</param>
        /// <param name="alias">The alias of the joining entity</param>
        /// <param name="source">The alias of the source entity</param>
        /// <returns>A IJoinQueryProvider{TJoin}</returns>
        IJoinQueryProvider<TJoin> Join<TJoin>(Expression<Func<TJoin, T, bool>> predicate, string alias = null, string source = null);

        /// <summary>
        /// Joines a new entity type to a previous entity
        /// </summary>
        /// <typeparam name="TJoin">The type to join</typeparam>
        /// <typeparam name="TOrig">The type of the previous entity to join to</typeparam>
        /// <param name="predicate">The expression that defines the connection</param>
        /// <param name="alias">The alias of the joining entity</param>
        /// <param name="source">The alias of the source entity</param>
        /// <returns>A IJoinQueryProvider{TJoin}</returns>
        IJoinQueryProvider<TJoin> Join<TJoin, TOrig>(Expression<Func<TJoin, TOrig, bool>> predicate, string alias = null, string source = null);

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type
        /// </summary>
        /// <typeparam name="TOut">The Property</typeparam>
        /// <param name="predicate">The expression that returns the Property</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryProvider<T> Map<TOut>(Expression<Func<T, TOut>> predicate);

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias defined (Table.Field as Alias)
        /// </summary>
        /// <typeparam name="TOut">The Property</typeparam>
        /// <param name="source">The expression that returns the Property</param>
        /// <param name="alias">The alias name the field will get (... as Alias)</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryProvider<T> Map<TOut>(Expression<Func<T, TOut>> source, string alias);

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias from the select type
        /// </summary>
        /// <typeparam name="TAlias">The select type containig the alias property</typeparam>
        /// <typeparam name="TOut">The alias Type</typeparam>
        /// <param name="source">The source expression returning the source property</param>
        /// <param name="alias">The select expression returning the alias property</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryProvider<T> Map<TAlias, TOut>(Expression<Func<T, TOut>> source, Expression<Func<TAlias, TOut>> alias);

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias from the select type
        /// </summary>
        /// <typeparam name="TSource">The select type containig the source alias property</typeparam>
        /// <typeparam name="TAlias">The select type containig the alias property</typeparam>
        /// <typeparam name="TOut">The alias Type</typeparam>
        /// <param name="source">The source expression returning the source property</param>
        /// <param name="alias">The select expression returning the alias property</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryProvider<T> Map<TSource, TAlias, TOut>(Expression<Func<TSource, TOut>> source, Expression<Func<TAlias, TOut>> alias);




        IWhereQueryProvider<T> Where(Expression<Func<T, bool>> predicate);

        IWhereQueryProvider<T> Where<T2>(Expression<Func<T2, bool>> predicate);

        //IWhereQueryProvider<T> Where<T2>(Expression<Func<T2, T, bool>> predicate);

        IWhereQueryProvider<T> Where<T2, T3>(Expression<Func<T2, T3, bool>> predicate);




        IOrderQueryProvider<T> OrderBy<TOrder>(Expression<Func<T, TOrder>> predicate);

        IOrderQueryProvider<T2> OrderBy<T2, TOrder>(Expression<Func<T2, TOrder>> predicate);

        IOrderQueryProvider<T> OrderByDesc<TOrder>(Expression<Func<T, TOrder>> predicate);

        IOrderQueryProvider<T2> OrderByDesc<T2, TOrder>(Expression<Func<T2, TOrder>> predicate);

        
    }

    public interface IJoinQueryProvider<T> : ISelectQueryProvider<T>, IQueryProvider
    {
        //IJoinQueryProvider<T> And<TAnd>(Expression<Func<T, TAnd, bool>> predicate);

        IJoinQueryProvider<T> And<TAnd>(Expression<Func<T, TAnd, bool>> predicate, string alias = null, string source = null);

        IJoinQueryProvider<T> Or<TOr>(Expression<Func<T, TOr, bool>> predicate, string alias = null, string source = null);
    }

    public interface IWhereQueryProvider<T> : ISelectQueryProviderBase<T>, IQueryProvider
    {
        IJoinQueryProvider<T> And(Expression<Func<T, bool>> predicate);

        IJoinQueryProvider<T> And<TAnd>(Expression<Func<TAnd, bool>> predicate, string alias = null);

        IJoinQueryProvider<T> And<TAnd>(Expression<Func<T, TAnd, bool>> predicate, string alias = null, string source = null);

        IJoinQueryProvider<T> And<TSource, TAnd>(Expression<Func<TSource, TAnd, bool>> predicate, string alias = null, string source = null);


        IJoinQueryProvider<T> Or(Expression<Func<T, bool>> predicate);

        IJoinQueryProvider<T> Or<TOr>(Expression<Func<TOr, bool>> predicate, string alias = null);

        IJoinQueryProvider<T> Or<TOr>(Expression<Func<T, TOr, bool>> predicate, string alias = null, string source = null);

        IJoinQueryProvider<T> Or<TSource, TOr>(Expression<Func<TSource, TOr, bool>> predicate, string alias = null, string source = null);



        IOrderQueryProvider<T> OrderBy<TOrder>(Expression<Func<T, TOrder>> predicate);

        IOrderQueryProvider<T2> OrderBy<T2, TOrder>(Expression<Func<T2, TOrder>> predicate);

        IOrderQueryProvider<T> OrderByDesc<TOrder>(Expression<Func<T, TOrder>> predicate);

        IOrderQueryProvider<T2> OrderByDesc<T2, TOrder>(Expression<Func<T2, TOrder>> predicate);
    }

    public interface IOrderQueryProvider<T> : ISelectQueryProviderBase<T>, IQueryProvider
    {
        IOrderQueryProvider<T> ThenBy<TOrder>(Expression<Func<T, TOrder>> predicate);

        IOrderQueryProvider<T> ThenBy<T2, TOrder>(Expression<Func<T2, TOrder>> predicate);

        IOrderQueryProvider<T> ThenByDesc<TOrder>(Expression<Func<T, TOrder>> predicate);

        IOrderQueryProvider<T> ThenByDesc<T2, TOrder>(Expression<Func<T2, TOrder>> predicate);
    }
}
