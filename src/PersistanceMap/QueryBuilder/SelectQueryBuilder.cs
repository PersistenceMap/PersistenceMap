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
            SelectQueryPartsBuilder.Instance.AppendSimpleQueryPart(QueryPartsMap, OperationType.Select);

            // add the from operation
            SelectQueryPartsBuilder.Instance.AppendEntityQueryPart<T2>(QueryPartsMap, OperationType.From);

            return new SelectQueryBuilder<T2>(Context, QueryPartsMap);
        }

        internal ISelectQueryExpression<T2> From<T2>(string alias)
        {
            alias.EnsureArgumentNotNullOrEmpty("alias");

            // create the begining for the select operation
            SelectQueryPartsBuilder.Instance.AppendSimpleQueryPart(QueryPartsMap, OperationType.Select);

            // add the from operation with a alias
            var part = SelectQueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.From);
            part.EntityAlias = alias;

            return new SelectQueryBuilder<T2>(Context, QueryPartsMap);
        }

        private SelectQueryBuilder<T2> CreateExpressionQueryPart<T2>(OperationType operation, LambdaExpression predicate)
        {
            SelectQueryPartsBuilder.Instance.AppendExpressionQueryPart(QueryPartsMap, predicate, operation);

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
    }
}
