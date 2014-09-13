using PersistanceMap.QueryBuilder;
using PersistanceMap.QueryParts;

namespace PersistanceMap.Test
{
    internal static class IQueryProviderExtensions
    {
        public static ISelectQueryProviderBase<TRebase> Rebase<T, TRebase>(this ISelectQueryProviderBase<T> query)
        {
            return new SelectQueryBuilder<TRebase>(query.Context, query.QueryPartsMap as SelectQueryPartsMap);
        }
    }
}
