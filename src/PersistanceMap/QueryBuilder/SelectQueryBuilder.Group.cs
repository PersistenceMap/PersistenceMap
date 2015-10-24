using PersistanceMap.Expressions;
using PersistanceMap.Factories;
using PersistanceMap.QueryParts;
using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public partial class SelectQueryBuilder<T> : IGroupQueryExpression<T>
    {
        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        IGroupQueryExpression<T> IGroupQueryExpression<T>.ThenBy(Expression<Func<T, object>> predicate)
        {
            return ((IGroupQueryExpression<T>)this).ThenBy<T>(predicate);
        }

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <typeparam name="T2">The type containing the member to group by</typeparam>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        IGroupQueryExpression<T> IGroupQueryExpression<T>.ThenBy<T2>(Expression<Func<T2, object>> predicate)
        {
            var field = predicate.TryExtractPropertyName();
            var part = new DelegateQueryPart(OperationType.ThenBy, () => field);
            QueryParts.Add(part);

            return new SelectQueryBuilder<T>(Context, QueryParts);
        }

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> IGroupQueryExpression<T>.OrderBy(Expression<Func<T, object>> predicate)
        {
            return OrderBy(predicate);
        }

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T2> IGroupQueryExpression<T>.OrderBy<T2>(Expression<Func<T2, object>> predicate)
        {
            return OrderBy<T2>(predicate);
        }

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> IGroupQueryExpression<T>.OrderByDesc(Expression<Func<T, object>> predicate)
        {
            return OrderByDesc(predicate);
        }

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T2> IGroupQueryExpression<T>.OrderByDesc<T2>(Expression<Func<T2, object>> predicate)
        {
            return OrderByDesc<T2>(predicate);
        }
    }
}
