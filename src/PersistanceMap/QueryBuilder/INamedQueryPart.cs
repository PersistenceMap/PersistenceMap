
namespace PersistanceMap.QueryBuilder
{
    public interface INamedQueryPart : IQueryPart
    {
        string Name { get; }
    }
}
