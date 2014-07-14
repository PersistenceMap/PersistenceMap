using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface ISelectExpression<T>
    {
        IEnumerable<T> Select<T>();

        IEnumerable<T> Select();
        
        T Single<T>();
        
        ISelectExpression<T> Join<TJoin>(Expression<Func<TJoin, T, bool>> predicate);

        ISelectExpression<T> Join<TJoin>(params Expression<Func<MapOption<TJoin, T>, IExpressionMapQueryPart>>[] args);


        ISelectExpression<T> Where(Expression<Func<T, bool>> predicate);

        ISelectExpression<T> Where<T2>(Expression<Func<T2, bool>> predicate);

        ISelectExpression<T> Where<T2, T3>(params Expression<Func<MapOption<T2, T3>, IExpressionMapQueryPart>>[] args);
    }
}
