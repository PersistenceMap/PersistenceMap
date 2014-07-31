﻿using PersistanceMap.Compiler;
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
    public class SelectQueryProvider<T> : ISelectQueryProvider<T>, IQueryProvider
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

        internal ISelectQueryProvider<T> Map(string source, string alias, string entity, string entityalias)
        {
            //TODO: is this the corect place to do this? shouldn't the QueryPart map its own children with the right alias?
            // if there is a alias on the last item it has to be used with the map

            var last = QueryPartsMap.Parts.Last(l => l.OperationType == OperationType.From || l.OperationType == OperationType.Join) as IEntityQueryPart;
            if (last != null && !string.IsNullOrEmpty(last.EntityAlias) && entity == last.Entity)
                entity = last.EntityAlias;

            QueryPartsFactory.AppendFieldQueryMap(QueryPartsMap, source, alias, entity, entityalias);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        #endregion

        #region ISqlExpression<T> Implementation

        #region Join Expressions

        public IJoinQueryProvider<TJoin> Join<TJoin>(Expression<Func<TJoin, T, bool>> predicate)
        {
            QueryPartsFactory.AppendEntityQueryPart<TJoin, T>(QueryPartsMap, predicate, OperationType.Join);

            return new JoinQueryProvider<TJoin>(Context, QueryPartsMap);
        }

        public IJoinQueryProvider<TJoin> Join<TJoin>(string alias, Expression<Func<TJoin, T, bool>> predicate)
        {
            var part = QueryPartsFactory.AppendEntityQueryPart<TJoin, T>(QueryPartsMap, predicate, OperationType.Join);
            part.EntityAlias = alias;

            foreach (var itm in part.Parts)
            {
                var map = itm as IExpressionQueryPart;
                if (map == null)
                    continue;

                // add aliases to mapcollections
                map.AliasMap.Add(typeof(TJoin), alias);
            }

            return new JoinQueryProvider<TJoin>(Context, QueryPartsMap);
        }

        public IJoinQueryProvider<TJoin> Join<TJoin>(string alias, string source, Expression<Func<TJoin, T, bool>> predicate)
        {
            var part = QueryPartsFactory.AppendEntityQueryPart<TJoin, T>(QueryPartsMap, predicate, OperationType.Join);
            part.EntityAlias = alias;

            foreach (var itm in part.Parts)
            {
                var map = itm as IExpressionQueryPart;
                if (map == null)
                    continue;

                // add aliases to mapcollections
                map.AliasMap.Add(typeof(T), source);
                map.AliasMap.Add(typeof(TJoin), alias);
            }

            return new JoinQueryProvider<TJoin>(Context, QueryPartsMap);
        }

        public IJoinQueryProvider<TJoin> Join<TJoin, T1>(Expression<Func<TJoin, T1, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Map Expressions

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type
        /// </summary>
        /// <typeparam name="T2">The Property</typeparam>
        /// <param name="predicate">The expression that returns the Property</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        public ISelectQueryProvider<T> Map<T2>(Expression<Func<T, T2>> predicate)
        {
            var source = FieldHelper.TryExtractPropertyName(predicate);
            var entity = typeof(T).Name;

            return Map(source, null, entity, entity);
        }

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias from the select type
        /// </summary>
        /// <typeparam name="TAlias">The select type containig the alias property</typeparam>
        /// <typeparam name="TOut">The alias Type</typeparam>
        /// <param name="source">The source expression returning the source property</param>
        /// <param name="alias">The select expression returning the alias property</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        public ISelectQueryProvider<T> Map<TAlias, TOut>(Expression<Func<T, TOut>> source, Expression<Func<TAlias, TOut>> alias)
        {
            var aliasField = FieldHelper.TryExtractPropertyName(alias);
            var sourceField = FieldHelper.TryExtractPropertyName(source);
            var entity = typeof(T).Name;

            return Map(sourceField, aliasField, entity, null);
        }

        public ISelectQueryProvider<T> Map<TSource, TAlias, TOut>(Expression<Func<TSource, TOut>> source, Expression<Func<TAlias, TOut>> alias)
        {
            var aliasField = FieldHelper.TryExtractPropertyName(alias);
            var sourceField = FieldHelper.TryExtractPropertyName(source);
            var entity = typeof(TSource).Name;

            return Map(sourceField, aliasField, entity, null);
        }

        public ISelectQueryProvider<T> Map<TOut>(Expression<Func<T, TOut>> source, string alias)
        {
            alias.EnsureArgumentNotNullOrEmpty("alias");

            var sourceField = FieldHelper.TryExtractPropertyName(source);
            var entity = typeof(T).Name;

            return Map(sourceField, alias, entity, null);
        }

        #endregion

        #region Where Expressions

        public IWhereQueryProvider<T> Where(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IWhereQueryProvider<T> Where<T2>(Expression<Func<T2, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IWhereQueryProvider<T> Where<T2>(Expression<Func<T, T2, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IWhereQueryProvider<T> Where<T2, T3>(Expression<Func<T2, T3, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Select Expressions

        public IEnumerable<T2> Select<T2>()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T2>(QueryPartsMap);

            return Context.Execute<T2>(query);
        }

        public IEnumerable<T> Select()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(QueryPartsMap);

            return Context.Execute<T>(query);
        }

        public T2 Single<T2>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Compiles the Query to a sql statement
        /// </summary>
        /// <typeparam name="T">The select type</typeparam>
        /// <returns>The sql string</returns>
        public string CompileQuery<T2>()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T2>(QueryPartsMap);

            return query.QueryString;
        }

        #endregion

        #endregion
    }

    internal class JoinQueryProvider<T> : SelectQueryProvider<T>, IJoinQueryProvider<T>
    {
        public JoinQueryProvider(IDatabaseContext context, SelectQueryPartsMap container)
            : base(context, container)
        {
        }

        #region IJoinQueryProvider Implementation

        public IJoinQueryProvider<T> And<TAnd>(Expression<Func<T, TAnd, bool>> predicate)
        {
            var part = new ExpressionQueryPart(OperationType.And, predicate);
            QueryPartsMap.AddToLast(part, p => p.OperationType == OperationType.Join || p.OperationType == OperationType.LeftJoin || p.OperationType == OperationType.FullJoin || p.OperationType == OperationType.RightJoin || p.OperationType == OperationType.Where);

            return new JoinQueryProvider<T>(Context, QueryPartsMap);
        }

        public IJoinQueryProvider<T> And<TAnd>(string source, string reference, Expression<Func<T, TAnd, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IJoinQueryProvider<T> Or<TOr>(Expression<Func<T, TOr, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
