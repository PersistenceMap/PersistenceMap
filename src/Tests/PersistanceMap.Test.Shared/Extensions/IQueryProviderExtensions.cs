using PersistanceMap.QueryBuilder;
using PersistanceMap.QueryParts;

namespace PersistanceMap.Test
{
    public static class IQueryProviderExtensions
    {
        public static ISelectQueryExpressionBase<TRebase> Rebase<T, TRebase>(this ISelectQueryExpressionBase<T> query)
        {
            return new SelectQueryBuilder<TRebase>(query.Context, query.QueryParts as SelectQueryPartsContainer);
        }
    }
}
