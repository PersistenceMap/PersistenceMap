using PersistenceMap.QueryBuilder;
using PersistenceMap.QueryParts;

namespace PersistenceMap.Test
{
    public static class IQueryProviderExtensions
    {
        public static ISelectQueryExpressionBase<TRebase> Rebase<T, TRebase>(this ISelectQueryExpressionBase<T> query)
        {
            return new SelectQueryBuilder<TRebase>(query.Context, query.QueryParts as SelectQueryPartsContainer);
        }
    }
}
