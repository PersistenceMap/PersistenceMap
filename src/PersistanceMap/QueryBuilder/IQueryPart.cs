
namespace PersistanceMap.QueryBuilder
{
    public interface IQueryPart
    {
        MapOperationType MapOperationType { get; }

        string Compile();
    }
}
