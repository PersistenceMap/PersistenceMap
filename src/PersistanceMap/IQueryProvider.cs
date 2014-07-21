
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public interface IQueryProvider
    {
        IDatabaseContext Context { get; }

        IQueryPartsMap QueryPartsMap { get; }

        //void Add(IQueryMap map);
    }
}
