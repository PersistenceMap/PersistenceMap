using PersistanceMap.QueryProvider;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public interface ISelectQueryProvider<T> : IQueryProvider
    {
        //ISelectQueryProvider<T> Join<TJoin>(params Expression<Func<IJoinMapOption<TJoin, T>, IQueryMap>>[] maps);

        //ISelectQueryProvider<T> Join<TJoin>(Expression<Func<IJoinMapOption<TJoin, T>, IQueryMap>> option);

        //ISelectQueryProvider<T> JoinOn<TJoin>(Expression<Func<TJoin, T, bool>> predicate);

        IJoinQueryProvider<TJoin> Join<TJoin>(Expression<Func<TJoin, T, bool>> predicate);

        IJoinQueryProvider<TJoin> Join<TJoin>(string alias, Expression<Func<TJoin, T, bool>> predicate);

        IJoinQueryProvider<TJoin> Join<TJoin>(string alias, string source, Expression<Func<TJoin, T, bool>> predicate);

        IJoinQueryProvider<TJoin> Join<TJoin, T1>(Expression<Func<TJoin, T1, bool>> predicate);






        ISelectQueryProvider<T> Include<T2>(Expression<Func<T, T2>> predicate);

        ISelectQueryProvider<T> Include<T2>(Expression<Func<T, T2>> predicate, string alias);

        ISelectQueryProvider<T> Map<TAlias, TOut>(Expression<Func<T, TOut>> source, Expression<Func<TAlias, TOut>> alias);

        ISelectQueryProvider<T> Map<TSource, TAlias, TOut>(Expression<Func<TSource, TOut>> source, Expression<Func<TAlias, TOut>> alias);

        ISelectQueryProvider<T> Map<TOut>(Expression<Func<T, TOut>> source, string alias);




        IWhereQueryProvider<T> Where(Expression<Func<T, bool>> predicate);

        IWhereQueryProvider<T> Where<T2>(Expression<Func<T2, bool>> predicate);

        IWhereQueryProvider<T> Where<T2>(Expression<Func<T, T2, bool>> predicate);

        IWhereQueryProvider<T> Where<T2, T3>(Expression<Func<T2, T3, bool>> predicate);
        //ISelectQueryProvider<T> Where<T2, T3>(params Expression<Func<IJoinMapOption<T2, T3>, IQueryMap>>[] maps);



        


        //TODO: Create base interface!
        IEnumerable<T2> Select<T2>();

        IEnumerable<T> Select();

        T2 Single<T2>();



        /// <summary>
        /// Compiles the Query to a sql statement
        /// </summary>
        /// <returns>The sql string</returns>
        string CompileQuery();
    }

    public interface IJoinQueryProvider<T> : ISelectQueryProvider<T>, IQueryProvider
    {
        IJoinQueryProvider<T> And<TAnd>(Expression<Func<T, TAnd, bool>> predicate);

        IJoinQueryProvider<T> Or<TOr>(Expression<Func<T, TOr, bool>> predicate);





        //IEnumerable<T2> Select<T2>();

        //IEnumerable<T> Select();

        //T2 Single<T2>();
    }

    public interface IWhereQueryProvider<T> : IQueryProvider
    {
        IJoinQueryProvider<T> And<TAnd>(Expression<Func<TAnd, bool>> predicate);

        IJoinQueryProvider<T> And<TAnd>(Expression<Func<T, TAnd, bool>> predicate);

        IJoinQueryProvider<T> And<TSource, TAnd>(Expression<Func<TSource, TAnd, bool>> predicate);

        IJoinQueryProvider<T> Or<TOr>(Expression<Func<T, TOr, bool>> predicate);

        IJoinQueryProvider<T> Or<TSource, TOr>(Expression<Func<T, TOr, bool>> predicate);




        //TODO: Create base interface!
        IEnumerable<T2> Select<T2>();

        IEnumerable<T> Select();

        T2 Single<T2>();
    }
}
