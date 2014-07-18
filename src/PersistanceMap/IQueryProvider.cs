
namespace PersistanceMap
{
    public interface IQueryProvider
    {
        IDatabaseContext Context { get; }

        IQueryPartsMap QueryPartsMap { get; }
    }
}
