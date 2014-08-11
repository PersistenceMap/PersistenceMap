using PersistanceMap.Compiler;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder;
using PersistanceMap.QueryBuilder.Decorators;
using System.Reflection;
using System.Linq;
using PersistanceMap.Internals;

namespace PersistanceMap.QueryProvider
{
    public partial class SelectQueryProvider<T> : ISelectQueryExpression<T>, ISelectQueryProvider<T>, IJoinQueryProvider<T>, IWhereQueryProvider<T>, IQueryProvider
    {
        public SelectQueryProvider(IDatabaseContext context)
        {
            _context = context;
        }

        public SelectQueryProvider(IDatabaseContext context, SelectQueryPartsMap container)
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

        IQueryPartsMap IQueryProvider.QueryPartsMap
        {
            get
            {
                return QueryPartsMap;
            }
        }

        #endregion

        #region Internal Implementation

        internal ISelectQueryProvider<T2> From<T2>()
        {
            // create the begining for the select operation
            QueryPartsFactory.AppendSelectMapQueryPart(QueryPartsMap, OperationType.SelectMap);

            // add the from operation
            QueryPartsFactory.AppendEntityQueryPart<T2>(QueryPartsMap, OperationType.From);

            return new SelectQueryProvider<T2>(Context, QueryPartsMap);
        }

        internal ISelectQueryProvider<T2> From<T2>(string alias)
        {
            alias.EnsureArgumentNotNullOrEmpty("alias");

            // create the begining for the select operation
            QueryPartsFactory.AppendSelectMapQueryPart(QueryPartsMap, OperationType.SelectMap);

            // add the from operation with a alias
            var part = QueryPartsFactory.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.From);
            part.EntityAlias = alias;

            return new SelectQueryProvider<T2>(Context, QueryPartsMap);
        }

        protected ISelectQueryProvider<T> Map(string source, string alias, string entity, string entityalias)
        {
            //TODO: is this the corect place to do this? shouldn't the QueryPart map its own children with the right alias?
            // if there is a alias on the last item it has to be used with the map

            var last = QueryPartsMap.Parts.Last(l => l.OperationType == OperationType.From || l.OperationType == OperationType.Join) as IEntityQueryPart;
            if (last != null && !string.IsNullOrEmpty(last.EntityAlias) && entity == last.Entity)
                entity = last.EntityAlias;

            QueryPartsFactory.AppendFieldQueryMap(QueryPartsMap, source, alias, entity, entityalias);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        /// <summary>
        /// Creates an ExpressionQueryPart and adds it to the last Join, LeftJoin, FullJoin, RightJoin or Where operation
        /// </summary>
        /// <param name="operation">The operationtype for the new querypart</param>
        /// <param name="predicate">The predicate containing the expression to execute</param>
        /// <returns>A new instance of selectqueryprovider containing a cumlated view of the complete expression</returns>
        protected IExpressionQueryPart AppendExpressionQueryPartToLast(OperationType operation, LambdaExpression predicate)
        {
            var part = new ExpressionQueryPart(operation, predicate);
            QueryPartsMap.AddToLast(part, p => p.OperationType == OperationType.Join || p.OperationType == OperationType.LeftJoin || p.OperationType == OperationType.FullJoin || p.OperationType == OperationType.RightJoin || p.OperationType == OperationType.Where);

            return part;
        }

        protected SelectQueryProvider<T2> CreateExpressionQueryPart<T2>(OperationType operation, LambdaExpression predicate)
        {
            QueryPartsFactory.AppendExpressionQueryPart(QueryPartsMap, predicate, operation);

            return new SelectQueryProvider<T2>(Context, QueryPartsMap);
        }

        protected IJoinQueryProvider<T1> CreateEntityQueryPart<T1, T2>(Expression<Func<T1, T2, bool>> predicate, OperationType operation, string alias = null, string source = null)
        {
            var part = QueryPartsFactory.AppendEntityQueryPart(QueryPartsMap, predicate, operation);
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

            return new SelectQueryProvider<T1>(Context, QueryPartsMap);
        }

        #endregion
    }
}
