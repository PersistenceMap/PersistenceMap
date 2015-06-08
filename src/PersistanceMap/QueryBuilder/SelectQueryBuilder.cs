using System;
using System.Linq.Expressions;
using PersistanceMap.QueryParts;
using PersistanceMap.Sql;
using PersistanceMap.Tracing;

namespace PersistanceMap.QueryBuilder
{
    public partial class SelectQueryBuilder<T> : ISelectQueryExpressionBase<T>, ISelectQueryExpression<T>, IJoinQueryExpression<T>, IWhereQueryExpression<T>, IAfterMapQueryExpression<T>, IQueryExpression
    {
        public SelectQueryBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public SelectQueryBuilder(IDatabaseContext context, SelectQueryPartsContainer container)
        {
            _context = context;
            _queryParts = container;
        }

        private ILogger _logger;
        protected ILogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = Context.Kernel.LoggerFactory.CreateLogger();
                return _logger;
            }
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

        SelectQueryPartsContainer _queryParts;
        public SelectQueryPartsContainer QueryParts
        {
            get
            {
                if (_queryParts == null)
                    _queryParts = new SelectQueryPartsContainer();
                return _queryParts;
            }
        }

        IQueryPartsContainer IQueryExpression.QueryParts
        {
            get
            {
                return QueryParts;
            }
        }

        #endregion

        #region Internal Implementation

        internal ISelectQueryExpression<T2> From<T2>()
        {
            // create the begining for the select operation
            var selectPart = new ItemsQueryPart(OperationType.Select);

            QueryParts.Add(selectPart);

            // add the from operation
            var entityPart = new EntityPart(OperationType.From, typeof(T2).Name, null);
            QueryParts.Add(entityPart);

            return new SelectQueryBuilder<T2>(Context, QueryParts);
        }

        internal ISelectQueryExpression<T2> From<T2>(string alias)
        {
            alias.EnsureArgumentNotNullOrEmpty("alias");

            // create the begining for the select operation
            var selectPart = new ItemsQueryPart(OperationType.Select);
            QueryParts.Add(selectPart);

            // add the from operation with a alias
            var entity = typeof(T).Name;
            var entityPart = new EntityPart(OperationType.From, entity, alias);
            QueryParts.Add(entityPart);

            return new SelectQueryBuilder<T2>(Context, QueryParts);
        }

        #endregion

        #region Private Generalized Implementation of Interfaces

        private SelectQueryBuilder<T> Or<TOr>(Expression<Func<T, TOr, bool>> operation, string alias = null, string source = null)
        {
            var partMap = new ExpressionAliasMap(operation);
            var part = new DelegateQueryPart(OperationType.Or, () => LambdaToSqlCompiler.Compile(partMap));
            QueryParts.Add(part);
            
            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                partMap.AliasMap.Add(typeof(T), alias);

            if (!string.IsNullOrEmpty(source))
                partMap.AliasMap.Add(typeof(TOr), source);

            return new SelectQueryBuilder<T>(Context, QueryParts);
        }

        private SelectQueryBuilder<T> And<TAnd>(Expression<Func<T, TAnd, bool>> operation, string alias = null, string source = null)
        {
            var partMap = new ExpressionAliasMap(operation);
            var part = new DelegateQueryPart(OperationType.And, () => LambdaToSqlCompiler.Compile(partMap));

            QueryParts.Add(part);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                partMap.AliasMap.Add(typeof(T), alias);

            if (!string.IsNullOrEmpty(source))
                partMap.AliasMap.Add(typeof(TAnd), source);

            return new SelectQueryBuilder<T>(Context, QueryParts);
        }



        private SelectQueryBuilder<T> ThenBy(Expression<Func<T, object>> predicate)
        {
            var part = new DelegateQueryPart(OperationType.ThenByAsc, () => LambdaToSqlCompiler.Compile(predicate));
            QueryParts.Add(part);

            return new SelectQueryBuilder<T>(Context, QueryParts);
        }

        private SelectQueryBuilder<T> ThenBy<T2>(Expression<Func<T2, object>> predicate)
        {
            var part = new DelegateQueryPart(OperationType.ThenByAsc, () => LambdaToSqlCompiler.Compile(predicate));
            QueryParts.Add(part);

            return new SelectQueryBuilder<T>(Context, QueryParts);
        }

        #endregion
    }
}
