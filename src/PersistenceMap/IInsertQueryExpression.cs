using System;
using System.Linq.Expressions;

namespace PersistenceMap
{
    public interface IInsertQueryExpression<T> : IQueryExpression
    {
        /// <summary>
        /// Marks a propterty not to be included in the insert statement
        /// </summary>
        /// <param name="predicate">The property to ignore</param>
        /// <returns></returns>
        IInsertQueryExpression<T> Ignore(Expression<Func<T, object>> predicate);
    }
}
