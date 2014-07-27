using PersistanceMap.QueryProvider;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public interface ISelectQueryProvider<T> : IQueryProvider
    {
        ISelectQueryProvider<T> Join<TJoin>(params Expression<Func<IJoinMapOption<TJoin, T>, IQueryMap>>[] args);

        ISelectQueryProvider<T> Join<TJoin>(Expression<Func<IJoinMapOption<TJoin, T>, IQueryMap>> option);

        ISelectQueryProvider<T> JoinOn<TJoin>(Expression<Func<TJoin, T, bool>> predicate);

        ISelectQueryProvider<T> Where(Expression<Func<T, bool>> predicate);

        ISelectQueryProvider<T> Where<T2>(Expression<Func<T2, bool>> predicate);

        ISelectQueryProvider<T> Where<T2, T3>(params Expression<Func<IJoinMapOption<T2, T3>, IQueryMap>>[] args);


        IEnumerable<T2> Select<T2>();

        IEnumerable<T> Select();

        IEnumerable<T2> Select<T2>(params Expression<Func<ISelectMapOption<T2>, IQueryMap>>[] mappings);

        //IEnumerable<T> Select(params Expression<Func<MapOption<T>, IQueryMap>>[] mappings);

        T2 Single<T2>();
    }
}
