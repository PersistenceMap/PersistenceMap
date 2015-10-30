using System;
using System.Linq.Expressions;
using PersistenceMap.QueryParts;
using PersistenceMap.Sql;
using PersistenceMap.Expressions;

namespace PersistenceMap.QueryBuilder
{
    public partial class WhereQueryBuilder<T> : SelectQueryBuilderBase<T>, IWhereQueryExpression<T>, ISelectQueryExpressionBase<T>, IQueryExpression
    {
        public WhereQueryBuilder(IDatabaseContext context) 
            : base(context)
        {
        }

        public WhereQueryBuilder(IDatabaseContext context, SelectQueryPartsContainer container) 
            : base(context, container)
        {
        }

        #region IWhereQueryProvider Implementation

        #region And Expressions

        public IWhereQueryExpression<T> And(Expression<Func<T, bool>> operation)
        {
            return And<T>(operation);
        }

        public IWhereQueryExpression<T> And<TAnd>(Expression<Func<TAnd, bool>> operation, string alias = null)
        {
            var partMap = new ExpressionAliasMap(operation);
            var part = new DelegateQueryPart(OperationType.And, () => LambdaToSqlCompiler.Compile(partMap).ToString(), typeof(T));
            QueryParts.Add(part);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                partMap.AliasMap.Add(typeof(TAnd), alias);

            return new WhereQueryBuilder<T>(Context, QueryParts);
        }

        public IWhereQueryExpression<T> And<TAnd>(Expression<Func<T, TAnd, bool>> operation, string alias = null, string source = null)
        {
            var partMap = new ExpressionAliasMap(operation);
            var part = new DelegateQueryPart(OperationType.And, () => LambdaToSqlCompiler.Compile(partMap), typeof(T));

            QueryParts.Add(part);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
            {
                partMap.AliasMap.Add(typeof(T), alias);
            }

            if (!string.IsNullOrEmpty(source))
            {
                partMap.AliasMap.Add(typeof(TAnd), source);
            }

            return new WhereQueryBuilder<T>(Context, QueryParts);
        }

        public IWhereQueryExpression<T> And<TSource, TAnd>(Expression<Func<TSource, TAnd, bool>> operation, string alias = null, string source = null)
        {
            var partMap = new ExpressionAliasMap(operation);
            var part = new DelegateQueryPart(OperationType.And, () => LambdaToSqlCompiler.Compile(partMap).ToString(), typeof(T));
            QueryParts.Add(part);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
            {
                partMap.AliasMap.Add(typeof(TSource), alias);
            }

            if (!string.IsNullOrEmpty(source))
            {
                partMap.AliasMap.Add(typeof(TAnd), source);
            }

            return new WhereQueryBuilder<T>(Context, QueryParts);
        }

        #endregion

        #region Or Expressions

        public IWhereQueryExpression<T> Or(Expression<Func<T, bool>> operation)
        {
            return Or<T>(operation);
        }

        public IWhereQueryExpression<T> Or<TOr>(Expression<Func<TOr, bool>> operation, string alias = null)
        {
            var partMap = new ExpressionAliasMap(operation);
            var part = new DelegateQueryPart(OperationType.Or, () => LambdaToSqlCompiler.Compile(partMap), typeof(T));
            QueryParts.Add(part);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
            {
                partMap.AliasMap.Add(typeof(TOr), alias);
            }

            return new WhereQueryBuilder<T>(Context, QueryParts);
        }
        
        public IWhereQueryExpression<T> Or<TOr>(Expression<Func<T, TOr, bool>> operation, string alias = null, string source = null)
        {
            var partMap = new ExpressionAliasMap(operation);
            var part = new DelegateQueryPart(OperationType.Or, () => LambdaToSqlCompiler.Compile(partMap), typeof(T));
            QueryParts.Add(part);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
            {
                partMap.AliasMap.Add(typeof(T), alias);
            }

            if (!string.IsNullOrEmpty(source))
            {
                partMap.AliasMap.Add(typeof(TOr), source);
            }

            return new WhereQueryBuilder<T>(Context, QueryParts);
        }

        public IWhereQueryExpression<T> Or<TSource, TOr>(Expression<Func<TSource, TOr, bool>> operation, string alias = null, string source = null)
        {
            var partMap = new ExpressionAliasMap(operation);
            var part = new DelegateQueryPart(OperationType.Or, () => LambdaToSqlCompiler.Compile(partMap), typeof(T));
            QueryParts.Add(part);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
            {
                partMap.AliasMap.Add(typeof(TSource), alias);
            }

            if (!string.IsNullOrEmpty(source))
            {
                partMap.AliasMap.Add(typeof(TOr), source);
            }

            return new WhereQueryBuilder<T>(Context, QueryParts);
        }

        #endregion

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
            // TODO: add table name?
            var part = new DelegateQueryPart(OperationType.OrderBy, () => LambdaToSqlCompiler.Instance.Compile(predicate).ToString(), typeof(T2));
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
            var part = new DelegateQueryPart(OperationType.OrderByDesc, () => LambdaToSqlCompiler.Instance.Compile(predicate).ToString(), typeof(T2));
            QueryParts.Add(part);

            return new OrderQueryBuilder<T2>(Context, QueryParts);
        }

        #endregion

        #region GroupBy Expressions

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        public IGroupQueryExpression<T> GroupBy(Expression<Func<T, object>> predicate)
        {
            return GroupBy<T>(predicate);
        }

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <typeparam name="T2">The type containing the member to group by</typeparam>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        public IGroupQueryExpression<T> GroupBy<T2>(Expression<Func<T2, object>> predicate)
        {
            // TODO: add table name?
            var field = predicate.TryExtractPropertyName();
            var part = new DelegateQueryPart(OperationType.GroupBy, () => field, typeof(T));
            QueryParts.Add(part);

            return new GroupQueryBuilder<T>(Context, QueryParts);
        }

        #endregion

        #endregion
    }
}