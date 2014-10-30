using System;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder.QueryPartsBuilders;
using PersistanceMap.QueryParts;

namespace PersistanceMap.QueryBuilder
{
    public partial class SelectQueryBuilder<T> : ISelectQueryExpressionBase<T>, ISelectQueryExpression<T>, IJoinQueryExpression<T>, IWhereQueryExpression<T>, IAfterMapQueryExpression<T>, IQueryExpression
    {
        public SelectQueryBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public SelectQueryBuilder(IDatabaseContext context, SelectQueryPartsMap container)
        {
            _context = context;
            _queryPartsMap = container;
        }

        #region IQueryProvider Implementation

        readonly IDatabaseContext _context;
        public IDatabaseContext Context
        {
            get
            {
                return _context;
            }
        }

        SelectQueryPartsMap _queryPartsMap;
        public SelectQueryPartsMap QueryPartsMap
        {
            get
            {
                if (_queryPartsMap == null)
                    _queryPartsMap = new SelectQueryPartsMap();
                return _queryPartsMap;
            }
        }

        IQueryPartsMap IQueryExpression.QueryPartsMap
        {
            get
            {
                return QueryPartsMap;
            }
        }

        #endregion

        #region Internal Implementation

        internal ISelectQueryExpression<T2> From<T2>()
        {
            // create the begining for the select operation
            //SelectQueryPartsBuilder.Instance.AppendSimpleQueryPart(QueryPartsMap, OperationType.Select);
            var selectPart = new DelegateQueryPart(OperationType.Select, () => "select ");
            QueryPartsMap.Add(selectPart);

            // add the from operation
            SelectQueryPartsBuilder.Instance.AppendEntityQueryPart<T2>(QueryPartsMap, OperationType.From);

            return new SelectQueryBuilder<T2>(Context, QueryPartsMap);
        }

        internal ISelectQueryExpression<T2> From<T2>(string alias)
        {
            alias.EnsureArgumentNotNullOrEmpty("alias");

            // create the begining for the select operation
            //SelectQueryPartsBuilder.Instance.AppendSimpleQueryPart(QueryPartsMap, OperationType.Select);
            var selectPart = new DelegateQueryPart(OperationType.Select, () => "select ");
            QueryPartsMap.Add(selectPart);

            // add the from operation with a alias
            var part = SelectQueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.From);
            part.EntityAlias = alias;

            return new SelectQueryBuilder<T2>(Context, QueryPartsMap);
        }

        private SelectQueryBuilder<T2> CreateExpressionQueryPart<T2>(OperationType operation, LambdaExpression predicate)
        {
            SelectQueryPartsBuilder.Instance.AddExpressionQueryPart(QueryPartsMap, predicate, operation);

            return new SelectQueryBuilder<T2>(Context, QueryPartsMap);
        }

        private IJoinQueryExpression<T1> CreateEntityQueryPart<T1, T2>(Expression<Func<T1, T2, bool>> predicate, OperationType operation, string alias = null, string source = null)
        {
            var part = SelectQueryPartsBuilder.Instance.AppendEntityQueryPart(QueryPartsMap, predicate, operation);
            if (!string.IsNullOrEmpty(alias))
                part.EntityAlias = alias;

            foreach (var itm in part.Parts)
            {
                var map = itm as IExpressionQueryPart;
                if (map == null)
                    continue;

                // add aliases to mapcollections
                if (!string.IsNullOrEmpty(source))
                    map.AliasMap.Add(typeof(T2), source);

                if (!string.IsNullOrEmpty(alias))
                    map.AliasMap.Add(typeof(T1), alias);
            }

            return new SelectQueryBuilder<T1>(Context, QueryPartsMap);
        }

        #endregion


        #region Private Generalized Implementation of Interfaces

        private SelectQueryBuilder<T> Or<TOr>(Expression<Func<T, TOr, bool>> operation, string alias = null, string source = null)
        {
            var part = SelectQueryPartsBuilder.Instance.AppendExpressionQueryPartToLast(Context, QueryPartsMap, OperationType.Or, operation);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                part.AliasMap.Add(typeof(T), alias);

            if (!string.IsNullOrEmpty(source))
                part.AliasMap.Add(typeof(TOr), source);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        private SelectQueryBuilder<T> And<TAnd>(Expression<Func<T, TAnd, bool>> operation, string alias = null, string source = null)
        {
            var part = SelectQueryPartsBuilder.Instance.AppendExpressionQueryPartToLast(Context, QueryPartsMap, OperationType.And, operation);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                part.AliasMap.Add(typeof(T), alias);

            if (!string.IsNullOrEmpty(source))
                part.AliasMap.Add(typeof(TAnd), source);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }



        private SelectQueryBuilder<T> ThenBy(Expression<Func<T, object>> predicate)
        {
            return CreateExpressionQueryPart<T>(OperationType.ThenBy, predicate);
        }

        private SelectQueryBuilder<T> ThenBy<T2>(Expression<Func<T2, object>> predicate)
        {
            return CreateExpressionQueryPart<T>(OperationType.ThenBy, predicate);
        }

        #endregion
    }
}
