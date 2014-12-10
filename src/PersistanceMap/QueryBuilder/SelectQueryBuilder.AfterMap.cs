using PersistanceMap.Factories;
using PersistanceMap.QueryParts;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public partial class SelectQueryBuilder<T> : IAfterMapQueryExpression<T>, IQueryExpression
    {
        /// <summary>
        /// Marks the provided field as ignored. The field will not be included in the select.
        /// </summary>
        /// <param name="predicate">Marks the member to ignore</param>
        /// <returns>IAfterMapQueryProvider{T}</returns>
        IAfterMapQueryExpression<T> IAfterMapQueryExpression<T>.Ignore(Expression<Func<T, object>> predicate)
        {
            return Ignore(predicate);
        }

        /// <summary>
        /// Maps the provided field to a specific table. This helps to avoid Ambiguous column errors.
        /// </summary>
        /// <typeparam name="TSource">The source Table to map the member from</typeparam>
        /// <param name="predicate">Marks the member to be mapped</param>
        /// <returns>IAfterMapQueryProvider{T}</returns>
        public IAfterMapQueryExpression<T> Map<TSource>(Expression<Func<TSource, object>> predicate)
        {
            foreach (var part in QueryPartsMap.Parts.Where(p => p.OperationType == OperationType.Select))
            {
                var map = part as IQueryPartDecorator;
                if (map == null)
                    continue;

                var fieldName = FieldHelper.TryExtractPropertyName(predicate);

                var subpart = map.Parts.OfType<IFieldMap>().FirstOrDefault(f => f.Field == fieldName || f.FieldAlias == fieldName);
                if (subpart != null)
                {
                    subpart.EntityAlias = typeof (TSource).Name;
                }
            }

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias from the select type
        /// </summary>
        /// <typeparam name="TAlias">The select type containig the alias property</typeparam>
        /// <param name="source">The source expression returning the source property</param>
        /// <param name="alias">The select expression returning the alias property</param>
        /// <param name="converter">The converter that converts the database value to the desired value in the dataobject</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        IAfterMapQueryExpression<T> IAfterMapQueryExpression<T>.Map<TAlias>(Expression<Func<TAlias, object>> source, Expression<Func<T, object>> alias = null, Expression<Func<object, object>> converter = null)
        {
            var aliasField = FieldHelper.TryExtractPropertyName(alias);
            var sourceField = FieldHelper.TryExtractPropertyName(source);
            var entity = typeof(TAlias).Name;

            return Map(sourceField, aliasField, entity, null, converter);
        }

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        IGroupQueryExpression<T> IAfterMapQueryExpression<T>.GroupBy(Expression<Func<T, object>> predicate)
        {
            return GroupBy(predicate);
        }

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <typeparam name="T2">The type containing the member to group by</typeparam>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        IGroupQueryExpression<T> IAfterMapQueryExpression<T>.GroupBy<T2>(Expression<Func<T2, object>> predicate)
        {
            return GroupBy<T2>(predicate);
        }
    }
}

