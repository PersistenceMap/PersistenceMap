using System;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder.QueryPartsBuilders;

namespace PersistanceMap.QueryBuilder
{
    public partial class SelectQueryBuilder<T> : IWhereQueryExpression<T>, IQueryExpression
    {
        #region Private Implementation

        private SelectQueryBuilder<T> Or<TOr>(Expression<Func<T, TOr, bool>> predicate, string alias = null, string source = null)
        {
            var part = SelectQueryPartsBuilder.Instance.AppendExpressionQueryPartToLast(Context, QueryPartsMap, OperationType.Or, predicate);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                part.AliasMap.Add(typeof(T), alias);

            if (!string.IsNullOrEmpty(source))
                part.AliasMap.Add(typeof(TOr), source);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        private SelectQueryBuilder<T> And<TAnd>(Expression<Func<T, TAnd, bool>> predicate, string alias = null, string source = null)
        {
            var part = SelectQueryPartsBuilder.Instance.AppendExpressionQueryPartToLast(Context, QueryPartsMap, OperationType.And, predicate);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                part.AliasMap.Add(typeof(T), alias);

            if (!string.IsNullOrEmpty(source))
                part.AliasMap.Add(typeof(TAnd), source);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        #endregion

        #region IWhereQueryProvider Implementation

        #region And Expressions

        public IWhereQueryExpression<T> And(Expression<Func<T, bool>> predicate)
        {
            return And<T>(predicate);
        }

        public IWhereQueryExpression<T> And<TAnd>(Expression<Func<TAnd, bool>> predicate, string alias = null)
        {
            var part = SelectQueryPartsBuilder.Instance.AppendExpressionQueryPartToLast(Context, QueryPartsMap, OperationType.And, predicate);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                part.AliasMap.Add(typeof(TAnd), alias);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        IWhereQueryExpression<T> IWhereQueryExpression<T>.And<TAnd>(Expression<Func<T, TAnd, bool>> predicate, string alias = null, string source = null)
        {
            return And<TAnd>(predicate, alias, source);
        }

        public IWhereQueryExpression<T> And<TSource, TAnd>(Expression<Func<TSource, TAnd, bool>> predicate, string alias = null, string source = null)
        {
            var part = SelectQueryPartsBuilder.Instance.AppendExpressionQueryPartToLast(Context, QueryPartsMap, OperationType.And, predicate);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                part.AliasMap.Add(typeof(TSource), alias);

            if (!string.IsNullOrEmpty(source))
                part.AliasMap.Add(typeof(TAnd), source);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        #endregion

        #region Or Expressions

        public IWhereQueryExpression<T> Or(Expression<Func<T, bool>> predicate)
        {
            return Or<T>(predicate);
        }

        public IWhereQueryExpression<T> Or<TOr>(Expression<Func<TOr, bool>> predicate, string alias = null)
        {
            var part = SelectQueryPartsBuilder.Instance.AppendExpressionQueryPartToLast(Context, QueryPartsMap, OperationType.Or, predicate);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                part.AliasMap.Add(typeof(TOr), alias);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        IWhereQueryExpression<T> IWhereQueryExpression<T>.Or<TOr>(Expression<Func<T, TOr, bool>> predicate, string alias, string source)
        {
            return Or<TOr>(predicate, alias, source);
        }

        public IWhereQueryExpression<T> Or<TSource, TOr>(Expression<Func<TSource, TOr, bool>> predicate, string alias = null, string source = null)
        {
            var part = SelectQueryPartsBuilder.Instance.AppendExpressionQueryPartToLast(Context, QueryPartsMap, OperationType.Or, predicate);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                part.AliasMap.Add(typeof(TSource), alias);

            if (!string.IsNullOrEmpty(source))
                part.AliasMap.Add(typeof(TOr), source);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        #endregion

        #region OrderBy Expressions

        IOrderQueryExpression<T> IWhereQueryExpression<T>.OrderBy(Expression<Func<T, object>> predicate)
        {
            return OrderBy(predicate);
        }

        IOrderQueryExpression<T2> IWhereQueryExpression<T>.OrderBy<T2>(Expression<Func<T2, object>> predicate)
        {
            return OrderBy<T2>(predicate);
        }

        IOrderQueryExpression<T> IWhereQueryExpression<T>.OrderByDesc(Expression<Func<T, object>> predicate)
        {
            return OrderByDesc(predicate);
        }

        IOrderQueryExpression<T2> IWhereQueryExpression<T>.OrderByDesc<T2>(Expression<Func<T2, object>> predicate)
        {
            return OrderByDesc<T2>(predicate);
        }
        
        #endregion

        #endregion

        

    }
}