using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.QueryProvider
{
    public partial class SelectQueryProvider<T> : IJoinQueryProvider<T>
    {
        #region ISelectQueryProvider<T> Implementation

        #region Join Expressions

        public IJoinQueryProvider<TJoin> Join<TJoin>(Expression<Func<TJoin, T, bool>> predicate)
        {
            QueryPartsFactory.AppendEntityQueryPart<TJoin, T>(QueryPartsMap, predicate, OperationType.Join);

            return new SelectQueryProvider<TJoin>(Context, QueryPartsMap);
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

            return new SelectQueryProvider<TJoin>(Context, QueryPartsMap);
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

            return new SelectQueryProvider<TJoin>(Context, QueryPartsMap);
        }

        public IJoinQueryProvider<TJoin> Join<TJoin, T1>(Expression<Func<TJoin, T1, bool>> predicate)
        {
            QueryPartsFactory.AppendEntityQueryPart<TJoin, T1>(QueryPartsMap, predicate, OperationType.Join);
            
            return new SelectQueryProvider<TJoin>(Context, QueryPartsMap);
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
            QueryPartsFactory.AppendExpressionQueryPart(QueryPartsMap, predicate, OperationType.Where);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        public IWhereQueryProvider<T> Where<T2>(Expression<Func<T2, bool>> predicate)
        {
            QueryPartsFactory.AppendExpressionQueryPart(QueryPartsMap, predicate, OperationType.Where);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        //public IWhereQueryProvider<T> Where<T2>(Expression<Func<T2, T, bool>> predicate)
        //{
        //    QueryPartsFactory.AppendExpressionQueryPart(QueryPartsMap, predicate, OperationType.Where);

        //    return new SelectQueryProvider<T>(Context, QueryPartsMap);
        //}

        public IWhereQueryProvider<T> Where<T2, T3>(Expression<Func<T2, T3, bool>> predicate)
        {
            QueryPartsFactory.AppendExpressionQueryPart(QueryPartsMap, predicate, OperationType.Where);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        #endregion

        #region OrderBy Expressions

        public IOrderQueryProvider<T> OrderBy<TOrder>(Expression<Func<T, TOrder>> predicate)
        {
            return AddExpressionQueryPart<T>(OperationType.OrderBy, predicate);
        }

        public IOrderQueryProvider<T2> OrderBy<T2, TOrder>(Expression<Func<T2, TOrder>> predicate)
        {
            return AddExpressionQueryPart<T2>(OperationType.OrderBy, predicate);
        }

        public IOrderQueryProvider<T> OrderByDesc<TOrder>(Expression<Func<T, TOrder>> predicate)
        {
            return AddExpressionQueryPart<T>(OperationType.OrderByDesc, predicate);
        }

        public IOrderQueryProvider<T2> OrderByDesc<T2, TOrder>(Expression<Func<T2, TOrder>> predicate)
        {
            return AddExpressionQueryPart<T2>(OperationType.OrderByDesc, predicate);
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
}
