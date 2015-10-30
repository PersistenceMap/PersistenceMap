using PersistenceMap.QueryParts;
using System;
using System.Linq.Expressions;
using PersistenceMap.Sql;

namespace PersistenceMap.QueryBuilder
{
    public class OrderQueryBuilder<T> : SelectQueryBuilderBase<T>, IOrderQueryExpression<T>, ISelectQueryExpressionBase<T>, IQueryExpression
    {
        public OrderQueryBuilder(IDatabaseContext context) 
            : base(context)
        {
        }

        public OrderQueryBuilder(IDatabaseContext context, SelectQueryPartsContainer container) 
            : base(context, container)
        {
        }
        
        #region OrderBy Expressions

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        public IOrderQueryExpression<T> ThenBy(Expression<Func<T, object>> predicate)
        {
            var part = new DelegateQueryPart(OperationType.ThenByAsc, () => LambdaToSqlCompiler.Compile(predicate), typeof(T));
            QueryParts.Add(part);

            return new OrderQueryBuilder<T>(Context, QueryParts);
        }

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        public IOrderQueryExpression<T> ThenBy<T2>(Expression<Func<T2, object>> predicate)
        {
            var part = new DelegateQueryPart(OperationType.ThenByAsc, () => LambdaToSqlCompiler.Compile(predicate), typeof(T));
            QueryParts.Add(part);

            return new OrderQueryBuilder<T>(Context, QueryParts);
        }

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        public IOrderQueryExpression<T> ThenByDesc(Expression<Func<T, object>> predicate)
        {
            var part = new DelegateQueryPart(OperationType.ThenByDesc, () => LambdaToSqlCompiler.Compile(predicate), typeof(T));
            QueryParts.Add(part);

            return new OrderQueryBuilder<T>(Context, QueryParts);
        }

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        public IOrderQueryExpression<T> ThenByDesc<T2>(Expression<Func<T2, object>> predicate)
        {
            var part = new DelegateQueryPart(OperationType.ThenByDesc, () => LambdaToSqlCompiler.Compile(predicate), typeof(T));
            QueryParts.Add(part);

            return new OrderQueryBuilder<T>(Context, QueryParts);
        }

        #endregion
    }
}


