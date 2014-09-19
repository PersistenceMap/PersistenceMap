using PersistanceMap.Internals;
using PersistanceMap.QueryParts;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public partial class SelectQueryBuilder<T> : IAfterMapQueryExpression<T>, IQueryProvider
    {
        public IAfterMapQueryExpression<T> AfterMap(Action<T> predicate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Marks the provided field as ignored. The field will not be included in the select.
        /// </summary>
        /// <param name="predicate">Marks the member to ignore</param>
        /// <returns>IAfterMapQueryProvider{T}</returns>
        public IAfterMapQueryExpression<T> Ignore(Expression<Func<T, object>> predicate)
        {
            foreach (var part in QueryPartsMap.Parts.Where(p => p.OperationType == OperationType.Select))
            {
                var map = part as IQueryPartDecorator;
                if (map == null)
                    continue;

                var fieldName = FieldHelper.TryExtractPropertyName(predicate);

                var subpart = map.Parts.FirstOrDefault(f => f is IFieldQueryPart && ((IFieldQueryPart)f).Field == fieldName || ((IFieldQueryPart)f).FieldAlias == fieldName);
                if (subpart != null)
                    map.Remove(subpart);
            }

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
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

                var subpart = map.Parts.FirstOrDefault(f => f is IFieldQueryPart && ((IFieldQueryPart)f).Field == fieldName || ((IFieldQueryPart)f).FieldAlias == fieldName) as IFieldQueryPart;
                if (subpart != null)
                {
                    subpart.EntityAlias = typeof (TSource).Name;
                }
            }

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
        }
    }
}

