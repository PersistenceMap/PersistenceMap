using PersistanceMap.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface ISelectExpression<T> : IPersistanceExpression
    {
        IEnumerable<T2> Select<T2>();

        IEnumerable<T> Select();
        
        T2 Single<T2>();
        
        ISelectExpression<T> Join<TJoin>(Expression<Func<TJoin, T, bool>> predicate);

        ISelectExpression<T> Join<TJoin>(params Expression<Func<SelectMapOption<TJoin, T>, IMapQueryPart>>[] args);

        ISelectExpression<T> Where(Expression<Func<T, bool>> predicate);

        ISelectExpression<T> Where<T2>(Expression<Func<T2, bool>> predicate);

        ISelectExpression<T> Where<T2, T3>(params Expression<Func<SelectMapOption<T2, T3>, IMapQueryPart>>[] args);
    }
}
