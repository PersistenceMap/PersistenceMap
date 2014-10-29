using PersistanceMap.Internals;
using PersistanceMap.QueryParts;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public partial class SelectQueryBuilder<T> : IAfterMapQueryExpression<T>, IQueryExpression
    {
        //public IAfterMapQueryExpression<T> AfterMap(Action<T> predicate)
        //{
        //    //QueryPartsMap.Add(new 


        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Marks the provided field as ignored. The field will not be included in the select.
        /// </summary>
        /// <param name="predicate">Marks the member to ignore</param>
        /// <returns>IAfterMapQueryProvider{T}</returns>
        IAfterMapQueryExpression<T> IAfterMapQueryExpression<T>.Ignore(Expression<Func<T, object>> predicate)
        {
            //foreach (var part in QueryPartsMap.Parts.Where(p => p.OperationType == OperationType.Select))
            //{
            //    var map = part as IQueryPartDecorator;
            //    if (map == null)
            //        continue;

            //    var fieldName = FieldHelper.TryExtractPropertyName(predicate);

            //    //var subpart = map.Parts.FirstOrDefault(f => f is IFieldQueryPart && ((IFieldQueryPart)f).Field == fieldName || ((IFieldQueryPart)f).FieldAlias == fieldName);
            //    var subpart = map.Parts.OfType<IFieldQueryPart>().FirstOrDefault(f => f.Field == fieldName || f.FieldAlias == fieldName);
            //    if (subpart != null)
            //        map.Remove(subpart);

            //    map.Add(new IgnoreFieldQueryPart(subpart.Field, subpart.FieldAlias));
            //}

            //return new SelectQueryBuilder<T>(Context, QueryPartsMap);

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

                var subpart = map.Parts.FirstOrDefault(f => f is IFieldQueryPart && ((IFieldQueryPart)f).Field == fieldName || ((IFieldQueryPart)f).FieldAlias == fieldName) as IFieldQueryPart;
                if (subpart != null)
                {
                    subpart.EntityAlias = typeof (TSource).Name;
                }
            }

            return new SelectQueryBuilder<T>(Context, QueryPartsMap);
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

