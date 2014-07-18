using PersistanceMap.QueryProvider;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public interface ISelectQueryProvider<T> : IQueryProvider
    {
        IEnumerable<T2> Select<T2>();

        IEnumerable<T> Select();
        
        T2 Single<T2>();
        
        ISelectQueryProvider<T> Join<TJoin>(Expression<Func<TJoin, T, bool>> predicate);

        ISelectQueryProvider<T> Join<TJoin>(params Expression<Func<SelectMapOption<TJoin, T>, IQueryMap>>[] args);

        ISelectQueryProvider<T> Where(Expression<Func<T, bool>> predicate);

        ISelectQueryProvider<T> Where<T2>(Expression<Func<T2, bool>> predicate);

        ISelectQueryProvider<T> Where<T2, T3>(params Expression<Func<SelectMapOption<T2, T3>, IQueryMap>>[] args);
    }
}
