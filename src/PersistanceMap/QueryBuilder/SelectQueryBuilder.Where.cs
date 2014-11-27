using System;
using System.Linq.Expressions;
using PersistanceMap.QueryParts;
using PersistanceMap.Sql;

namespace PersistanceMap.QueryBuilder
{
    public partial class SelectQueryBuilder<T> : IWhereQueryExpression<T>, IQueryExpression
    {
        #region IWhereQueryProvider Implementation

        #region And Expressions

        public IWhereQueryExpression<T> And(Expression<Func<T, bool>> operation)
        {
            return And<T>(operation);
        }

        public IWhereQueryExpression<T> And<TAnd>(Expression<Func<TAnd, bool>> operation, string alias = null)
        {
            var partMap = new ExpressionPart(operation);
            var part = new DelegateQueryPart(OperationType.And, () => string.Format("AND {0} ", LambdaToSqlCompiler.Compile(partMap)));
            QueryPartsMap.Add(part);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                partMap.AliasMap.Add(typeof(TAnd), alias);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        IWhereQueryExpression<T> IWhereQueryExpression<T>.And<TAnd>(Expression<Func<T, TAnd, bool>> operation, string alias = null, string source = null)
        {
            return And<TAnd>(operation, alias, source);
        }

        public IWhereQueryExpression<T> And<TSource, TAnd>(Expression<Func<TSource, TAnd, bool>> operation, string alias = null, string source = null)
        {
            var partMap = new ExpressionPart(operation);
            var part = new DelegateQueryPart(OperationType.And, () => string.Format("AND {0} ", LambdaToSqlCompiler.Compile(partMap)));
            QueryPartsMap.Add(part);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                partMap.AliasMap.Add(typeof(TSource), alias);

            if (!string.IsNullOrEmpty(source))
                partMap.AliasMap.Add(typeof(TAnd), source);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        #endregion

        #region Or Expressions

        public IWhereQueryExpression<T> Or(Expression<Func<T, bool>> operation)
        {
            return Or<T>(operation);
        }

        public IWhereQueryExpression<T> Or<TOr>(Expression<Func<TOr, bool>> operation, string alias = null)
        {
            var partMap = new ExpressionPart(operation);
            var part = new DelegateQueryPart(OperationType.Or, () => string.Format("OR {0} ", LambdaToSqlCompiler.Compile(partMap)));
            QueryPartsMap.Add(part);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                partMap.AliasMap.Add(typeof(TOr), alias);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        IWhereQueryExpression<T> IWhereQueryExpression<T>.Or<TOr>(Expression<Func<T, TOr, bool>> operation, string alias, string source)
        {
            return Or<TOr>(operation, alias, source);
        }

        public IWhereQueryExpression<T> Or<TSource, TOr>(Expression<Func<TSource, TOr, bool>> operation, string alias = null, string source = null)
        {
            var partMap = new ExpressionPart(operation);
            var part = new DelegateQueryPart(OperationType.Or, () => string.Format("OR {0} ", LambdaToSqlCompiler.Compile(partMap)));
            QueryPartsMap.Add(part);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                partMap.AliasMap.Add(typeof(TSource), alias);

            if (!string.IsNullOrEmpty(source))
                partMap.AliasMap.Add(typeof(TOr), source);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        #endregion

        #region OrderBy Expressions

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> IWhereQueryExpression<T>.OrderBy(Expression<Func<T, object>> predicate)
        {
            return OrderBy(predicate);
        }

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T2> IWhereQueryExpression<T>.OrderBy<T2>(Expression<Func<T2, object>> predicate)
        {
            return OrderBy<T2>(predicate);
        }

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T> IWhereQueryExpression<T>.OrderByDesc(Expression<Func<T, object>> predicate)
        {
            return OrderByDesc(predicate);
        }

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        IOrderQueryExpression<T2> IWhereQueryExpression<T>.OrderByDesc<T2>(Expression<Func<T2, object>> predicate)
        {
            return OrderByDesc<T2>(predicate);
        }
        
        #endregion

        #endregion

        

    }
}