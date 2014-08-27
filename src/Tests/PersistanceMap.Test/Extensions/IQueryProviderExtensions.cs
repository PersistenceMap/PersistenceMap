using PersistanceMap.QueryProvider;

namespace PersistanceMap.Test
{
    internal static class IQueryProviderExtensions
    {
        public static ISelectQueryProviderBase<TRebase> Rebase<T, TRebase>(this ISelectQueryProviderBase<T> query)
        {
            return new SelectQueryProvider<TRebase>(query.Context, query.QueryPartsMap as SelectQueryPartsMap);
        }
    }
}
