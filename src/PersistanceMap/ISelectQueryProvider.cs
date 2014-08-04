using PersistanceMap.QueryProvider;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public interface ISelectQueryProvider<T> : IQueryProvider
    {
        IJoinQueryProvider<TJoin> Join<TJoin>(Expression<Func<TJoin, T, bool>> predicate);

        IJoinQueryProvider<TJoin> Join<TJoin>(string alias, Expression<Func<TJoin, T, bool>> predicate);

        IJoinQueryProvider<TJoin> Join<TJoin>(string alias, string source, Expression<Func<TJoin, T, bool>> predicate);
        
        IJoinQueryProvider<TJoin> Join<TJoin, T1>(Expression<Func<TJoin, T1, bool>> predicate);





        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type
        /// </summary>
        /// <typeparam name="T2">The Property</typeparam>
        /// <param name="predicate">The expression that returns the Property</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryProvider<T> Map<T2>(Expression<Func<T, T2>> predicate);

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias from the select type
        /// </summary>
        /// <typeparam name="TAlias">The select type containig the alias property</typeparam>
        /// <typeparam name="TOut">The alias Type</typeparam>
        /// <param name="source">The source expression returning the source property</param>
        /// <param name="alias">The select expression returning the alias property</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        ISelectQueryProvider<T> Map<TAlias, TOut>(Expression<Func<T, TOut>> source, Expression<Func<TAlias, TOut>> alias);

        ISelectQueryProvider<T> Map<TSource, TAlias, TOut>(Expression<Func<TSource, TOut>> source, Expression<Func<TAlias, TOut>> alias);

        ISelectQueryProvider<T> Map<TOut>(Expression<Func<T, TOut>> source, string alias);




        IWhereQueryProvider<T> Where(Expression<Func<T, bool>> predicate);

        IWhereQueryProvider<T> Where<T2>(Expression<Func<T2, bool>> predicate);

        //IWhereQueryProvider<T> Where<T2>(Expression<Func<T2, T, bool>> predicate);

        IWhereQueryProvider<T> Where<T2, T3>(Expression<Func<T2, T3, bool>> predicate);



        


        //TODO: Create base interface!
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

    public interface IJoinQueryProvider<T> : ISelectQueryProvider<T>, IQueryProvider
    {
        IJoinQueryProvider<T> And<TAnd>(Expression<Func<T, TAnd, bool>> predicate);

        IJoinQueryProvider<T> And<TAnd>(string source, string reference, Expression<Func<T, TAnd, bool>> predicate);

        IJoinQueryProvider<T> Or<TOr>(Expression<Func<T, TOr, bool>> predicate);





        //IEnumerable<T2> Select<T2>();

        //IEnumerable<T> Select();

        //T2 Single<T2>();

    }

    public interface IWhereQueryProvider<T> : IQueryProvider
    {
        IJoinQueryProvider<T> And(Expression<Func<T, bool>> predicate);

        IJoinQueryProvider<T> And<TAnd>(Expression<Func<TAnd, bool>> predicate);

        IJoinQueryProvider<T> And<TAnd>(Expression<Func<T, TAnd, bool>> predicate);

        IJoinQueryProvider<T> And<TSource, TAnd>(Expression<Func<TSource, TAnd, bool>> predicate);


        IJoinQueryProvider<T> Or(Expression<Func<T, bool>> predicate);

        IJoinQueryProvider<T> Or<TOr>(Expression<Func<TOr, bool>> predicate);

        IJoinQueryProvider<T> Or<TOr>(Expression<Func<T, TOr, bool>> predicate);

        IJoinQueryProvider<T> Or<TSource, TOr>(Expression<Func<T, TOr, bool>> predicate);




        //TODO: Create base interface!
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
}
