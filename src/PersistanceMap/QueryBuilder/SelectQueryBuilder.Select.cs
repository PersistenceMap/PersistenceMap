using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder.QueryPartsBuilders;
using PersistanceMap.QueryParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public partial class SelectQueryBuilder<T> : IJoinQueryExpression<T>
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
        public IJoinQueryExpression<TJoin> Join<TJoin>(Expression<Func<TJoin, T, bool>> predicate, string alias = null, string source = null)
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
        public IJoinQueryExpression<TJoin> Join<TJoin, TOrig>(Expression<Func<TJoin, TOrig, bool>> predicate, string alias = null, string source = null)
        {
            return CreateEntityQueryPart(predicate, OperationType.Join, alias, source);
        }

        #endregion

        #region Map Expressions

        protected ISelectQueryExpression<T> Map(string source, string alias, string entity, string entityalias)
        {
            //TODO: is this the corect place to do this? shouldn't the QueryPart map its own children with the right alias?
            // if there is a alias on the last item it has to be used with the map

            var last = QueryPartsMap.Parts.Last(l => l.OperationType == OperationType.From || l.OperationType == OperationType.Join) as IEntityQueryPart;
            if (last != null && string.IsNullOrEmpty(last.EntityAlias) == false && entity == last.Entity)
                entity = last.EntityAlias;

            SelectQueryPartsBuilder.Instance.AddFieldQueryMap(QueryPartsMap, source, alias, entity, entityalias);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }


        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type
        /// </summary>
        /// <param name="predicate">The expression that returns the Property</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        public ISelectQueryExpression<T> Map(Expression<Func<T, object>> predicate)
        {
            var source = FieldHelper.TryExtractPropertyName(predicate);
            var entity = typeof(T).Name;

            return Map(source, null, entity, entity);
        }

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias defined (Table.Field as Alias)
        /// </summary>
        /// <param name="source">The expression that returns the Property</param>
        /// <param name="alias">The alias name the field will get (... as Alias)</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        public ISelectQueryExpression<T> Map(Expression<Func<T, object>> source, string alias)
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
        /// <param name="source">The source expression returning the source property</param>
        /// <param name="alias">The select expression returning the alias property</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        public ISelectQueryExpression<T> Map<TAlias>(Expression<Func<T, object>> source, Expression<Func<TAlias, object>> alias)
        {
            return Map<T, TAlias>(source, alias);
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
        public ISelectQueryExpression<T> Map<TSource, TAlias>(Expression<Func<TSource, object>> source, Expression<Func<TAlias, object>> alias)
        {
            var aliasField = FieldHelper.TryExtractPropertyName(alias);
            var sourceField = FieldHelper.TryExtractPropertyName(source);
            var entity = typeof(TSource).Name;

            return Map(sourceField, aliasField, entity, null);
        }

        #endregion

        //#region For Member

        //public ISelectQueryProvider<T> ForMember(Expression<Func<T, object>> predicate, Action<IMemberConfiguration> memberExpression)
        //{
        //    throw new NotImplementedException();
        //}

        //public ISelectQueryProvider<T> ForMember<TType>(Expression<Func<TType, object>> predicate, Action<IMemberConfiguration> memberExpression)
        //{
        //    throw new NotImplementedException();
        //}

        //#endregion

        #region Where Expressions

        public IWhereQueryExpression<T> Where(Expression<Func<T, bool>> predicate)
        {
            var part = SelectQueryPartsBuilder.Instance.AddExpressionQueryPart(QueryPartsMap, predicate, OperationType.Where);

            // check if the last part that was added containes a alias
            var last = QueryPartsMap.Parts.Last(l => 
                l.OperationType == OperationType.From || 
                l.OperationType == OperationType.Join ||
                l.OperationType == OperationType.FullJoin ||
                l.OperationType == OperationType.LeftJoin ||
                l.OperationType == OperationType.RightJoin) as IEntityQueryPart;

            if (last != null && !string.IsNullOrEmpty(last.EntityAlias) && last.Entity == typeof(T).Name)
                part.AliasMap.Add(typeof(T), last.EntityAlias);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        public IWhereQueryExpression<T> Where<T2>(Expression<Func<T2, bool>> predicate)
        {
            SelectQueryPartsBuilder.Instance.AddExpressionQueryPart(QueryPartsMap, predicate, OperationType.Where);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        public IWhereQueryExpression<T> Where<T2, T3>(Expression<Func<T2, T3, bool>> predicate)
        {
            SelectQueryPartsBuilder.Instance.AddExpressionQueryPart(QueryPartsMap, predicate, OperationType.Where);

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        #endregion

        #region OrderBy Expressions

        public IOrderQueryExpression<T> OrderBy(Expression<Func<T, object>> predicate)
        {
            return CreateExpressionQueryPart<T>(OperationType.OrderBy, predicate);
        }

        public IOrderQueryExpression<T2> OrderBy<T2>(Expression<Func<T2, object>> predicate)
        {
            return CreateExpressionQueryPart<T2>(OperationType.OrderBy, predicate);
        }

        public IOrderQueryExpression<T> OrderByDesc(Expression<Func<T, object>> predicate)
        {
            return CreateExpressionQueryPart<T>(OperationType.OrderByDesc, predicate);
        }

        public IOrderQueryExpression<T2> OrderByDesc<T2>(Expression<Func<T2, object>> predicate)
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

        public IEnumerable<TSelect> Select<TSelect>(Expression<Func<TSelect>> anonym)
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<TSelect>(QueryPartsMap);

            return Context.Execute<TSelect>(query);
        }

        public IEnumerable<TSelect> Select<TSelect>(Expression<Func<T, TSelect>> anonym)
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(QueryPartsMap);

            var elements = Context.Execute<T>(query);
            var expression = anonym.Compile();

            foreach (var item in elements)
            {
                yield return expression.Invoke(item);
            }
        }

        //public T2 Single<T2>()
        //{
        //    throw new NotImplementedException();
        //}

        public IAfterMapQueryExpression<TNew> For<TNew>()
        {
            var members = typeof(TNew).GetSelectionMembers();
            var fields = members.Select(m => m.ToFieldQueryPart(null, null));

            SelectQueryPartsBuilder.Instance.AddFiedlParts(QueryPartsMap, fields.ToArray());

            foreach (var part in QueryPartsMap.Parts.Where(p => p.OperationType == OperationType.Select))
            {
                // seal part to disalow other parts to be added to selectmaps
                var map = part as IQueryPartDecorator;
                if (map != null)
                    map.IsSealded = true;
            }

            return new SelectQueryBuilder<TNew>(Context, QueryPartsMap);
        }

        public IAfterMapQueryExpression<TAno> For<TAno>(Expression<Func<TAno>> anonym)
        {
            //throw new NotImplementedException("For has to make sure that the resultset values equals the defined type");
            //return new SelectQueryProvider<TAno>(Context, QueryPartsMap);
            return For<TAno>();
        }

        /// <summary>
        /// Compiles the Query to a sql statement for the given type
        /// </summary>
        /// <typeparam name="T">The select type</typeparam>
        /// <returns>The sql string</returns>
        public string CompileQuery<T2>()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T2>(QueryPartsMap);

            return query.QueryString;
        }

        /// <summary>
        /// Compiles the Query to a sql statement
        /// </summary>
        /// <returns>The sql string</returns>
        public string CompileQuery()
        {
            return CompileQuery<T>();
        }

        #endregion

        #endregion
    }
}
