using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap
{
    public interface ISelectExpression<T>
    {
        IEnumerable<T> Select<T>();

        IEnumerable<T> Select();

        T Single<T>();

        T Single();

        ISelectExpression<T> Join<TJoin>(Expression<Func<TJoin, T, bool>> predicate);

        //ISelectExpression<T> Join<TJoin>(params IExpressionMapQueryPart[] args);

        ISelectExpression<T> Join<TJoin>(params Expression<Func<MapOption<TJoin, T>, IExpressionMapQueryPart>>[] args);


        ISelectExpression<T> Where(Expression<Func<T, bool>> predicate);

        ISelectExpression<T> Where(params IExpressionMapQueryPart[] args);
    }
}
