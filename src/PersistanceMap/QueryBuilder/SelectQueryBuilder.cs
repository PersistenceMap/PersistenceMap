using System;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder.QueryPartsBuilders;
using PersistanceMap.QueryParts;
using PersistanceMap.Sql;

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
            var selectPart = new DelegateQueryPart(OperationType.Select, () => "SELECT ");
            QueryPartsMap.Add(selectPart);

            // add the from operation
            var entityPart = new DelegateQueryPart(OperationType.From, () => string.Format("FROM {0} ", typeof(T2).Name));
            QueryPartsMap.Add(entityPart);


            return new SelectQueryBuilder<T2>(Context, QueryPartsMap);
        }

        internal ISelectQueryExpression<T2> From<T2>(string alias)
        {
            alias.EnsureArgumentNotNullOrEmpty("alias");

            // create the begining for the select operation
            var selectPart = new DelegateQueryPart(OperationType.Select, () => "SELECT ");
            QueryPartsMap.Add(selectPart);

            // add the from operation with a alias
            var entity = typeof (T).Name;
            var entityPart = new EntityDelegateQueryPart(OperationType.From, () => string.Format("FROM {0}{1} ", entity, string.IsNullOrEmpty(alias) ? string.Empty : string.Format(" {0}", alias)), entity, alias);
            QueryPartsMap.Add(entityPart);

            return new SelectQueryBuilder<T2>(Context, QueryPartsMap);
        }

        #endregion


        #region Private Generalized Implementation of Interfaces

        private SelectQueryBuilder<T> Or<TOr>(Expression<Func<T, TOr, bool>> operation, string alias = null, string source = null)
        {
            var partMap = new ExpressionAliasMap(operation);
            var part = new DelegateQueryPart(OperationType.Or, () => string.Format("OR {0} ", LambdaToSqlCompiler.Compile(partMap)));
            QueryPartsMap.Add(part);
            
            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                partMap.AliasMap.Add(typeof(T), alias);

            if (!string.IsNullOrEmpty(source))
                partMap.AliasMap.Add(typeof(TOr), source);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        private SelectQueryBuilder<T> And<TAnd>(Expression<Func<T, TAnd, bool>> operation, string alias = null, string source = null)
        {
            var partMap = new ExpressionAliasMap(operation);
            var part = new DelegateQueryPart(OperationType.And, () => string.Format("AND {0} ", LambdaToSqlCompiler.Compile(partMap)));
            QueryPartsMap.Add(part);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                partMap.AliasMap.Add(typeof(T), alias);

            if (!string.IsNullOrEmpty(source))
                partMap.AliasMap.Add(typeof(TAnd), source);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }



        private SelectQueryBuilder<T> ThenBy(Expression<Func<T, object>> predicate)
        {
            var part = new DelegateQueryPart(OperationType.ThenBy, () => string.Format(", {0} ASC", LambdaToSqlCompiler.Instance.Compile(predicate)));
            QueryPartsMap.Add(part);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        private SelectQueryBuilder<T> ThenBy<T2>(Expression<Func<T2, object>> predicate)
        {
            var part = new DelegateQueryPart(OperationType.ThenBy, () => string.Format(", {0} ASC", LambdaToSqlCompiler.Instance.Compile(predicate)));
            QueryPartsMap.Add(part);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        #endregion
    }
}
