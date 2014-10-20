using System;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface IInsertQueryExpression<T> : IQueryExpression
    {
        //IInsertQueryExpression<T> AddToStore();

        //IInsertQueryExpression Insert<T>(Expression<Func<T>> dataObject);

        //IInsertQueryExpression Insert<T>(Expression<Func<object>> anonym);

        //IInsertQueryExpression Ignore<T>(Expression<Func<T, object>> predicate);

        IInsertQueryExpression<T> Ignore(Expression<Func<T, object>> predicate);
    }
}
