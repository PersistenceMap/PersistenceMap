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
    public partial class SelectQueryProvider<T> : IAfterMapQueryProvider<T>, IQueryProvider
    {
        public IAfterMapQueryProvider<T> AfterMap(Action<T> predicate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Marks the provided field as ignored. The field will not be included in the select.
        /// </summary>
        /// <param name="predicate">Marks the member to ignore</param>
        /// <returns>IAfterMapQueryProvider{T}</returns>
        public IAfterMapQueryProvider<T> Ignore(Expression<Func<T, object>> predicate)
        {
            foreach (var part in QueryPartsMap.Parts.Where(p => p.OperationType == OperationType.SelectMap))
            {
                var map = part as IQueryPartDecorator;
                if (map == null)
                    continue;

                var fieldName = FieldHelper.TryExtractPropertyName(predicate);

                var subpart = map.Parts.FirstOrDefault(f => f is IFieldQueryMap && ((IFieldQueryMap)f).Field == fieldName || ((IFieldQueryMap)f).FieldAlias == fieldName);
                if (subpart != null)
                    map.Remove(subpart);
            }

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        /// <summary>
        /// Maps the provided field to a specific table. This helps to avoid Ambiguous column errors.
        /// </summary>
        /// <typeparam name="TSource">The source Table to map the member from</typeparam>
        /// <param name="predicate">Marks the member to be mapped</param>
        /// <returns>IAfterMapQueryProvider{T}</returns>
        public IAfterMapQueryProvider<T> Map<TSource>(Expression<Func<TSource, object>> predicate)
        {
            foreach (var part in QueryPartsMap.Parts.Where(p => p.OperationType == OperationType.SelectMap))
            {
                var map = part as IQueryPartDecorator;
                if (map == null)
                    continue;

                var fieldName = FieldHelper.TryExtractPropertyName(predicate);

                var subpart = map.Parts.FirstOrDefault(f => f is IFieldQueryMap && ((IFieldQueryMap)f).Field == fieldName || ((IFieldQueryMap)f).FieldAlias == fieldName) as IFieldQueryMap;
                if (subpart != null)
                {
                    subpart.EntityAlias = typeof (TSource).Name;
                }
            }

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }
    }
}

