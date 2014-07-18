
namespace PersistanceMap
{
    public interface IPersistanceExpression
    {
        IDatabaseContext Context { get; }

        IQueryPartsMap QueryPartsMap { get; }
    }
}
