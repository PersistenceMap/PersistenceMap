using System;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface IUpdateQueryExpression<T> : IQueryExpression
    {
        /// <summary>
        /// Marks a property not to be included in the update statement
        /// </summary>
        /// <param name="predicate">The property to ignore</param>
        /// <returns></returns>
        IUpdateQueryExpression<T> Ignore(Expression<Func<T, object>> predicate);
    }
}
