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
            //QueryPartsMap.Add(typeof(T2).ToFromQueryPart<T>(QueryPartsMap));

            return new SelectQueryProvider<T2>(Context, QueryPartsMap);
        }

        internal ISelectQueryProvider<T2> From<T2>(params IQueryMap[] parts)
        {
            parts.EnsureArgumentNotNull("part");

            QueryPartsFactory.CreateEntityQueryPart<T>(QueryPartsMap, parts, MapOperationType.From);
            //QueryPartsMap.Add(typeof(T2).ToFromQueryPart<T>(QueryPartsMap, parts))

            return new SelectQueryProvider<T2>(Context, QueryPartsMap);
        }

        #endregion

        #region ISqlExpression<T> Implementation

        public ISelectQueryProvider<T> Join<TJoin>(params Expression<Func<IJoinMapOption<TJoin, T>, IQueryMap>>[] maps)
        {
            QueryPartsFactory.CreateEntityQueryPart<TJoin>(QueryPartsMap, QueryMapCompiler.Compile(maps).ToArray(), MapOperationType.Join);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        public ISelectQueryProvider<T> Join<TJoin>(Expression<Func<IJoinMapOption<TJoin, T>, IQueryMap>> option)
        {
            QueryPartsFactory.CreateEntityQueryPart<TJoin>(QueryPartsMap, QueryMapCompiler.Compile(option).ToArray(), MapOperationType.Join);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        public ISelectQueryProvider<T> JoinOn<TJoin>(Expression<Func<TJoin, T, bool>> predicate)
        {
            QueryPartsFactory.CreateEntityQueryPart<TJoin, T>(QueryPartsMap, predicate, MapOperationType.Join);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        




        public ISelectQueryProvider<T> Where(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ISelectQueryProvider<T> Where<T2>(Expression<Func<T2, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ISelectQueryProvider<T> Where<T2, T3>(params Expression<Func<IJoinMapOption<T2, T3>, IQueryMap>>[] maps)
        {
            throw new NotImplementedException();
        }



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

        public IEnumerable<T2> Select<T2>(params Expression<Func<ISelectMapOption<T2>, IQueryMap>>[] maps)
        {
            var parts = QueryMapCompiler.Compile(maps);

            parts.Where(p => p.MapOperationType == MapOperationType.Include).ForEach(part =>
            {
                var field = part as IFieldQueryMap;
                if (field == null)
                {
                    //var field = new FieldQueryPart(FieldHelper.TryExtractPropertyName(part.Expression), string.IsNullOrEmpty(entity.EntityAlias) ? entity.Entity : entity.EntityAlias, entity.Entity)
                    field = new FieldQueryPart(FieldHelper.TryExtractPropertyName(part.Expression), null, null)
                    {
                        MapOperationType = MapOperationType.Include
                    };
                }

                QueryPartsMap.Add(field);
            });

            return Select<T2>();
        }




        public T2 Single<T2>()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
