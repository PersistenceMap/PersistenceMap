using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap
{
    public interface IInsertQueryExpression<T> : IQueryExpression
    {
        IInsertQueryExpression<T> AddToStore();

        //IInsertQueryExpression Insert<T>(Expression<Func<T>> dataObject);

        //IInsertQueryExpression Insert<T>(Expression<Func<object>> anonym);

        //IInsertQueryExpression Ignore<T>(Expression<Func<T, object>> predicate);

        IInsertQueryExpression<T> Ignore(Expression<Func<T, object>> predicate);
    }
}
