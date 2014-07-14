
namespace PersistanceMap
{
    public interface IPersistanceExpression
    {
        IDbContext Context { get; }

        IQueryPartsMap QueryPartsMap { get; }
    }
}
