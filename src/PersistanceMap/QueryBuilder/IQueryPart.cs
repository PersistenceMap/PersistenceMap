
namespace PersistanceMap.QueryBuilder
{
    public interface IQueryPart
    {
        OperationType OperationType { get; }

        string Compile();
    }
}
