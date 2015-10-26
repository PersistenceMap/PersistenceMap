using PersistanceMap.Expressions;
using PersistanceMap.QueryParts;
using PersistanceMap.Sql;
using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public class GroupQueryBuilder<T> : SelectQueryBuilderBase<T>, IGroupQueryExpression<T>, ISelectQueryExpressionBase<T>, IQueryExpression
    {
        public GroupQueryBuilder(IDatabaseContext context) 
            : base(context)
        {
        }

        public GroupQueryBuilder(IDatabaseContext context, SelectQueryPartsContainer container) 
            : base(context, container)
        {
        }

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        public IGroupQueryExpression<T> ThenBy(Expression<Func<T, object>> predicate)
        {
            return ThenBy<T>(predicate);
        }

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <typeparam name="T2">The type containing the member to group by</typeparam>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        public IGroupQueryExpression<T> ThenBy<T2>(Expression<Func<T2, object>> predicate)
        {
            var field = predicate.TryExtractPropertyName();
            var part = new DelegateQueryPart(OperationType.ThenBy, () => field);
            QueryParts.Add(part);

            return new GroupQueryBuilder<T>(Context, QueryParts);
        }

        #region OrderBy Expressions

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        public IOrderQueryExpression<T> OrderBy(Expression<Func<T, object>> predicate)
        {
            return OrderBy<T>(predicate);
        }
        
        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        public IOrderQueryExpression<T2> OrderBy<T2>(Expression<Func<T2, object>> predicate)
        {
            //TODO: add table name?
            var part = new DelegateQueryPart(OperationType.OrderBy, () => LambdaToSqlCompiler.Instance.Compile(predicate).ToString());
            QueryParts.Add(part);

            return new OrderQueryBuilder<T2>(Context, QueryParts);
        }

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        public IOrderQueryExpression<T> OrderByDesc(Expression<Func<T, object>> predicate)
        {
            return OrderByDesc<T>(predicate);
        }

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        public IOrderQueryExpression<T2> OrderByDesc<T2>(Expression<Func<T2, object>> predicate)
        {
            var part = new DelegateQueryPart(OperationType.OrderByDesc, () => LambdaToSqlCompiler.Instance.Compile(predicate).ToString());
            QueryParts.Add(part);

            return new OrderQueryBuilder<T2>(Context, QueryParts);
        }

        #endregion
    }
}
