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
            QueryPartsFactory.CreateEntityQueryPart<T>(QueryPartsMap, MapOperationType.From);

            return new SelectQueryProvider<T2>(Context, QueryPartsMap);
        }

        //internal ISelectQueryProvider<T2> From<T2>(params IQueryMap[] parts)
        //{
        //    parts.EnsureArgumentNotNull("part");

        //    QueryPartsFactory.CreateEntityQueryPart<T>(QueryPartsMap, parts, MapOperationType.From);
        //    //QueryPartsMap.Add(typeof(T2).ToFromQueryPart<T>(QueryPartsMap, parts))

        //    return new SelectQueryProvider<T2>(Context, QueryPartsMap);
        //}

        internal ISelectQueryProvider<T2> From<T2>(string alias)
        {
            alias.EnsureArgumentNotNullOrEmpty("alias");

            //new QueryMap(MapOperationType.As, predicate)

            var part = QueryPartsFactory.CreateEntityQueryPart<T>(QueryPartsMap, MapOperationType.From);
            part.EntityAlias = alias;

            //QueryPartsMap.Add(typeof(T2).ToFromQueryPart<T>(QueryPartsMap, parts))

            return new SelectQueryProvider<T2>(Context, QueryPartsMap);
        }

        #endregion

        #region ISqlExpression<T> Implementation

        public IJoinQueryProvider<TJoin> Join<TJoin>(Expression<Func<TJoin, T, bool>> predicate)
        {
            QueryPartsFactory.CreateEntityQueryPart<TJoin, T>(QueryPartsMap, predicate, MapOperationType.Join);

            return new JoinQueryProvider<TJoin>(Context, QueryPartsMap);
        }

        public IJoinQueryProvider<TJoin> Join<TJoin>(string alias, Expression<Func<TJoin, T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IJoinQueryProvider<TJoin> Join<TJoin>(string alias, string source, Expression<Func<TJoin, T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IJoinQueryProvider<TJoin> Join<TJoin, T1>(Expression<Func<TJoin, T1, bool>> predicate)
        {
            throw new NotImplementedException();
        }




        public ISelectQueryProvider<T> Map<T2>(Expression<Func<T, T2>> predicate)
        {
            var part = FieldHelper.TryExtractPropertyName(predicate);
            var entity = typeof(T).Name;

            var field = new FieldQueryPart(part, entity, entity)
            {
                MapOperationType = MapOperationType.Include
            };

            QueryPartsMap.Add(field);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        //public ISelectQueryProvider<T> Include<T2>(Expression<Func<T, T2>> predicate, string alias)
        //{
        //    alias.EnsureArgumentNotNullOrEmpty("alias");

        //    var part = FieldHelper.TryExtractPropertyName(predicate);
        //    var entity = typeof(T).Name;

        //    var field = new FieldQueryPart(part, alias, entity)
        //    {
        //        MapOperationType = MapOperationType.Include
        //    };

        //    QueryPartsMap.Add(field);

        //    return new SelectQueryProvider<T>(Context, QueryPartsMap);
        //}

        public ISelectQueryProvider<T> Map<TAlias, TOut>(Expression<Func<T, TOut>> source, Expression<Func<TAlias, TOut>> alias)
        {
            var aliasField = FieldHelper.TryExtractPropertyName(alias);
            var sourceField = FieldHelper.TryExtractPropertyName(source);

            // create a expression that returns the field with a alias
            var entity = typeof(T).Name;
            var field = new FieldQueryPart(sourceField, aliasField, null /*EntityAlias*/, entity)
            {
                MapOperationType = MapOperationType.Include
            };

            QueryPartsMap.Add(field);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        public ISelectQueryProvider<T> Map<TSource, TAlias, TOut>(Expression<Func<TSource, TOut>> source, Expression<Func<TAlias, TOut>> alias)
        {
            var aliasField = FieldHelper.TryExtractPropertyName(alias);
            var sourceField = FieldHelper.TryExtractPropertyName(source);

            // create a expression that returns the field with a alias
            var entity = typeof(T).Name;
            var field = new FieldQueryPart(sourceField, aliasField, null /*EntityAlias*/, entity)
            {
                MapOperationType = MapOperationType.Include
            };

            QueryPartsMap.Add(field);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        public ISelectQueryProvider<T> Map<TOut>(Expression<Func<T, TOut>> source, string alias)
        {
            alias.EnsureArgumentNotNullOrEmpty("alias");

            var part = FieldHelper.TryExtractPropertyName(source);
            var entity = typeof(T).Name;

            var field = new FieldQueryPart(part, alias, entity)
            {
                MapOperationType = MapOperationType.Include
            };

            QueryPartsMap.Add(field);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }






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

        //public ISelectQueryProvider<T> Where<T2, T3>(params Expression<Func<IJoinMapOption<T2, T3>, IQueryMap>>[] maps)
        //{
        //    throw new NotImplementedException();
        //}



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

        //public IEnumerable<T2> Select<T2>(params Expression<Func<ISelectMapOption<T2>, IQueryMap>>[] maps)
        //{
        //    var parts = QueryMapCompiler.Compile(maps);

        //    parts.Where(p => p.MapOperationType == MapOperationType.Include).ForEach(part =>
        //    {
        //        var field = part as IFieldQueryMap;
        //        if (field == null)
        //        {
        //            //var field = new FieldQueryPart(FieldHelper.TryExtractPropertyName(part.Expression), string.IsNullOrEmpty(entity.EntityAlias) ? entity.Entity : entity.EntityAlias, entity.Entity)
        //            field = new FieldQueryPart(FieldHelper.TryExtractPropertyName(part.Expression), null, null)
        //            {
        //                MapOperationType = MapOperationType.Include
        //            };
        //        }

        //        QueryPartsMap.Add(field);
        //    });

        //    return Select<T2>();
        //}

        public T2 Single<T2>()
        {
            throw new NotImplementedException();
        }




        /// <summary>
        /// Compiles the Query to a sql statement
        /// </summary>
        /// <returns>The sql string</returns>
        public string CompileQuery()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(QueryPartsMap);

            return query.QueryString;
        }

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
            throw new NotImplementedException();
        }

        public IJoinQueryProvider<T> Or<TOr>(Expression<Func<T, TOr, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        //public IJoinQueryProvider<TJoin> Include<TJoin, T>(Expression<Func<TJoin, T>> predicate)
        //{
        //    throw new NotImplementedException();
        //}

        //public IJoinQueryProvider<T> Map<TMap>(Expression<Func<T, TMap, bool>> predicate)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion
    }
}
