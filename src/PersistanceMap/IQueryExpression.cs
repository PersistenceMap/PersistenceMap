
namespace PersistanceMap
{
    public interface IQueryExpression
    {
        IDatabaseContext Context { get; }

        IQueryPartsMap QueryPartsMap { get; }

        //void Add(IQueryMap map);
    }
}
