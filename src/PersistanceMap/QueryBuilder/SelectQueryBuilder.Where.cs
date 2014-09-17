using System;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder.QueryPartsBuilders;

namespace PersistanceMap.QueryBuilder
{
    public partial class SelectQueryBuilder<T> : IWhereQueryProvider<T>, IQueryProvider
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

        public IWhereQueryProvider<T> And(Expression<Func<T, bool>> predicate)
        {
            return And<T>(predicate);
        }

        public IWhereQueryProvider<T> And<TAnd>(Expression<Func<TAnd, bool>> predicate, string alias = null)
        {
            var part = SelectQueryPartsBuilder.Instance.AppendExpressionQueryPartToLast(Context, QueryPartsMap, OperationType.And, predicate);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                part.AliasMap.Add(typeof(TAnd), alias);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        IWhereQueryProvider<T> IWhereQueryProvider<T>.And<TAnd>(Expression<Func<T, TAnd, bool>> predicate, string alias = null, string source = null)
        {
            return And<TAnd>(predicate, alias, source);
        }

        public IWhereQueryProvider<T> And<TSource, TAnd>(Expression<Func<TSource, TAnd, bool>> predicate, string alias = null, string source = null)
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

        public IWhereQueryProvider<T> Or(Expression<Func<T, bool>> predicate)
        {
            return Or<T>(predicate);
        }

        public IWhereQueryProvider<T> Or<TOr>(Expression<Func<TOr, bool>> predicate, string alias = null)
        {
            var part = SelectQueryPartsBuilder.Instance.AppendExpressionQueryPartToLast(Context, QueryPartsMap, OperationType.Or, predicate);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                part.AliasMap.Add(typeof(TOr), alias);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        IWhereQueryProvider<T> IWhereQueryProvider<T>.Or<TOr>(Expression<Func<T, TOr, bool>> predicate, string alias, string source)
        {
            return Or<TOr>(predicate, alias, source);
        }

        public IWhereQueryProvider<T> Or<TSource, TOr>(Expression<Func<TSource, TOr, bool>> predicate, string alias = null, string source = null)
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

        IOrderQueryProvider<T> IWhereQueryProvider<T>.OrderBy(Expression<Func<T, object>> predicate)
        {
            return OrderBy(predicate);
        }

        IOrderQueryProvider<T2> IWhereQueryProvider<T>.OrderBy<T2>(Expression<Func<T2, object>> predicate)
        {
            return OrderBy<T2>(predicate);
        }

        IOrderQueryProvider<T> IWhereQueryProvider<T>.OrderByDesc(Expression<Func<T, object>> predicate)
        {
            return OrderByDesc(predicate);
        }

        IOrderQueryProvider<T2> IWhereQueryProvider<T>.OrderByDesc<T2>(Expression<Func<T2, object>> predicate)
        {
            return OrderByDesc<T2>(predicate);
        }
        
        #endregion

        #endregion

        

    }
}