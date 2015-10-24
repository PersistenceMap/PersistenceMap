using PersistanceMap.Expressions;
using PersistanceMap.QueryParts;
using PersistanceMap.Sql;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public class AfterMapQueryBuilder<T> : SelectQueryBuilderBase<T>, IAfterMapQueryExpression<T>, ISelectQueryExpressionBase<T>, IQueryExpression
    {
        public AfterMapQueryBuilder(IDatabaseContext context) 
            : base(context)
        {
        }

        public AfterMapQueryBuilder(IDatabaseContext context, SelectQueryPartsContainer container) 
            : base(context, container)
        {
        }

        /// <summary>
        /// Marks the provided field as ignored. The field will not be included in the select.
        /// </summary>
        /// <param name="predicate">Marks the member to ignore</param>
        /// <returns>IAfterMapQueryProvider{T}</returns>
        public IAfterMapQueryExpression<T> Ignore(Expression<Func<T, object>> predicate)
        {
            foreach (var part in QueryParts.Parts.Where(p => p.OperationType == OperationType.Select))
            {
                var map = part as IItemsQueryPart;
                if (map == null)
                    continue;

                var fieldName = LambdaExtensions.TryExtractPropertyName(predicate);

                // remove all previous mappings of the ignored field
                var subparts = map.Parts.OfType<IFieldPart>().Where(f => f.Field == fieldName || f.FieldAlias == fieldName).OfType<IQueryPart>();
                foreach (var subpart in subparts.ToList())
                {
                    map.Remove(subpart);
                }

                // add a field marked as ignored
                map.Add(new IgnoreFieldQueryPart(fieldName, string.Empty));
            }

            return new AfterMapQueryBuilder<T>(Context, QueryParts);
        }

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias from the select type
        /// </summary>
        /// <typeparam name="TSource">The select type containig the source property</typeparam>
        /// <param name="source">The source expression returning the source property</param>
        /// <param name="alias">The select expression returning the alias property</param>
        /// <param name="converter">The converter that converts the database value to the desired value in the dataobject</param>
        /// <returns>IAfterMapQueryProvider{T} containing the maps</returns>
        public IAfterMapQueryExpression<T> Map<TSource>(Expression<Func<TSource, object>> source, Expression<Func<T, object>> alias = null, Expression<Func<object, object>> converter = null)
        {
            string aliasField = null;
            if(alias != null)
                aliasField = alias.TryExtractPropertyName();

            var sourceField = source.TryExtractPropertyName();
            var sourceType = source.TryExtractPropertyType();
            var entity = typeof(TSource).Name;

            return Map(sourceField, aliasField, entity, null, converter, sourceType);
        }

        protected IAfterMapQueryExpression<T> Map(string source, string alias, string entity, string entityalias, Expression<Func<object, object>> converter, Type fieldType)
        {
            // if there is a alias on the last item it has to be used with the map
            var last = QueryParts.Parts.Where(l => l.OperationType == OperationType.From || l.OperationType == OperationType.Join).OfType<IEntityPart>().LastOrDefault();
            if (last != null && !string.IsNullOrEmpty(last.EntityAlias) && entity == last.Entity)
                entity = last.EntityAlias;

            // make sure the select part is not sealed so the custom map can be added
            bool isSealed = false;
            var parent = QueryParts.Parts.OfType<IItemsQueryPart>().LastOrDefault(p => p.OperationType == OperationType.Select);
            if (parent != null)
            {
                isSealed = parent.IsSealed;
                parent.IsSealed = false;

                var duplicate = parent.Parts.FirstOrDefault(p => p.ID == (alias ?? source));
                if (duplicate != null)
                    parent.Remove(duplicate);
            }

            var part = new FieldQueryPart(source, alias, entityalias, entity, alias ?? source, converter)
            {
                FieldType = fieldType,
                OperationType = OperationType.Include
            };
            QueryParts.Add(part);

            if (parent != null)
            {
                parent.IsSealed = isSealed;
            }

            return new AfterMapQueryBuilder<T>(Context, QueryParts);
        }
        
        #region GroupBy Expressions

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        public IGroupQueryExpression<T> GroupBy(Expression<Func<T, object>> predicate)
        {
            return GroupBy<T>(predicate);
        }

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <typeparam name="T2">The type containing the member to group by</typeparam>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        public IGroupQueryExpression<T> GroupBy<T2>(Expression<Func<T2, object>> predicate)
        {
            //TODO: add table name?
            var field = predicate.TryExtractPropertyName();
            var part = new DelegateQueryPart(OperationType.GroupBy, () => field);
            QueryParts.Add(part);

            return new GroupQueryBuilder<T>(Context, QueryParts);
        }

        #endregion

        #region OrderBy Expressions

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        public IOrderQueryExpression<T> OrderBy(Expression<Func<T, object>> predicate)
        {
            return OrderBy<T>(predicate);
        }

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        public IOrderQueryExpression<T2> OrderBy<T2>(Expression<Func<T2, object>> predicate)
        {
            //TODO: add table name?
            var part = new DelegateQueryPart(OperationType.OrderBy, () => LambdaToSqlCompiler.Instance.Compile(predicate).ToString());
            QueryParts.Add(part);

            return new OrderQueryBuilder<T2>(Context, QueryParts);
        }

        #endregion
    }
}

