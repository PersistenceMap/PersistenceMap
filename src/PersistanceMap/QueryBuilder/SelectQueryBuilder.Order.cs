using PersistanceMap.QueryParts;
using System;
using System.Linq.Expressions;
using PersistanceMap.Sql;

namespace PersistanceMap.QueryBuilder
{
    public partial class SelectQueryBuilder<T> : IOrderQueryExpression<T>, ISelectQueryExpressionBase<T>, IQueryExpression
    {
        #region OrderBy Expressions

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> IOrderQueryExpression<T>.ThenBy(Expression<Func<T, object>> predicate)
        {
            return ThenBy(predicate);
        }

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> IOrderQueryExpression<T>.ThenBy<T2>(Expression<Func<T2, object>> predicate)
        {
            return ThenBy<T2>(predicate);
        }

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        public IOrderQueryExpression<T> ThenByDesc(Expression<Func<T, object>> predicate)
        {
            var part = new DelegateQueryPart(OperationType.ThenByDesc, () => string.Format(", {0} DESC", LambdaToSqlCompiler.Instance.Compile(predicate)));
            QueryParts.Add(part);

            return new SelectQueryBuilder<T>(Context, QueryParts);
        }

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        public IOrderQueryExpression<T> ThenByDesc<T2>(Expression<Func<T2, object>> predicate)
        {
            var part = new DelegateQueryPart(OperationType.ThenByDesc, () => string.Format(", {0} DESC", LambdaToSqlCompiler.Instance.Compile(predicate)));
            QueryParts.Add(part);

            return new SelectQueryBuilder<T>(Context, QueryParts);
        }

        #endregion
    }
}


