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

        /// <summary>
        /// Joines a new entity type to the last entity
        /// </summary>
        /// <typeparam name="TJoin">The type to join</typeparam>
        /// <param name="predicate">The expression that defines the connection</param>
        /// <param name="alias">The alias of the joining entity</param>
        /// <param name="source">The alias of the source entity</param>
        /// <returns>A IJoinQueryProvider{TJoin}</returns>
        public IJoinQueryProvider<TJoin> Join<TJoin>(Expression<Func<TJoin, T, bool>> predicate, string alias = null, string source = null)
        {
            return CreateEntityQueryPart(predicate, OperationType.Join, alias, source);
        }

        /// <summary>
        /// Joines a new entity type to a previous entity
        /// </summary>
        /// <typeparam name="TJoin">The type to join</typeparam>
        /// <typeparam name="TOrig">The type of the previous entity to join to</typeparam>
        /// <param name="predicate">The expression that defines the connection</param>
        /// <param name="alias">The alias of the joining entity</param>
        /// <param name="source">The alias of the source entity</param>
        /// <returns>A IJoinQueryProvider{TJoin}</returns>
        public IJoinQueryProvider<TJoin> Join<TJoin, TOrig>(Expression<Func<TJoin, TOrig, bool>> predicate, string alias = null, string source = null)
        {
            return CreateEntityQueryPart(predicate, OperationType.Join, alias, source);
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
        /// Map a Property that is included in the result that belongs to a joined type with an alias defined (Table.Field as Alias)
        /// </summary>
        /// <typeparam name="TOut">The Property</typeparam>
        /// <param name="source">The expression that returns the Property</param>
        /// <param name="alias">The alias name the field will get (... as Alias)</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        public ISelectQueryProvider<T> Map<TOut>(Expression<Func<T, TOut>> source, string alias)
        {
            alias.EnsureArgumentNotNullOrEmpty("alias");

            var sourceField = FieldHelper.TryExtractPropertyName(source);
            var entity = typeof(T).Name;

            return Map(sourceField, alias, entity, null);
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
            return Map<T, TAlias, TOut>(source, alias);
        }

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias from the select type
        /// </summary>
        /// <typeparam name="TSource">The select type containig the source alias property</typeparam>
        /// <typeparam name="TAlias">The select type containig the alias property</typeparam>
        /// <typeparam name="TOut">The alias Type</typeparam>
        /// <param name="source">The source expression returning the source property</param>
        /// <param name="alias">The select expression returning the alias property</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        public ISelectQueryProvider<T> Map<TSource, TAlias, TOut>(Expression<Func<TSource, TOut>> source, Expression<Func<TAlias, TOut>> alias)
        {
            var aliasField = FieldHelper.TryExtractPropertyName(alias);
            var sourceField = FieldHelper.TryExtractPropertyName(source);
            var entity = typeof(TSource).Name;

            return Map(sourceField, aliasField, entity, null);
        }

        #endregion

        #region Where Expressions

        public IWhereQueryProvider<T> Where(Expression<Func<T, bool>> predicate)
        {
            var part = QueryPartsFactory.AppendExpressionQueryPart(QueryPartsMap, predicate, OperationType.Where);

            // check if the last part that was added containes a alias
            var last = QueryPartsMap.Parts.Last(l => 
                l.OperationType == OperationType.From || 
                l.OperationType == OperationType.Join ||
                l.OperationType == OperationType.FullJoin ||
                l.OperationType == OperationType.LeftJoin ||
                l.OperationType == OperationType.RightJoin) as IEntityQueryPart;

            if (last != null && !string.IsNullOrEmpty(last.EntityAlias) && last.Entity == typeof(T).Name)
                part.AliasMap.Add(typeof(T), last.EntityAlias);

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
            return CreateExpressionQueryPart<T>(OperationType.OrderBy, predicate);
        }

        public IOrderQueryProvider<T2> OrderBy<T2, TOrder>(Expression<Func<T2, TOrder>> predicate)
        {
            return CreateExpressionQueryPart<T2>(OperationType.OrderBy, predicate);
        }

        public IOrderQueryProvider<T> OrderByDesc<TOrder>(Expression<Func<T, TOrder>> predicate)
        {
            return CreateExpressionQueryPart<T>(OperationType.OrderByDesc, predicate);
        }

        public IOrderQueryProvider<T2> OrderByDesc<T2, TOrder>(Expression<Func<T2, TOrder>> predicate)
        {
            return CreateExpressionQueryPart<T2>(OperationType.OrderByDesc, predicate);
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
