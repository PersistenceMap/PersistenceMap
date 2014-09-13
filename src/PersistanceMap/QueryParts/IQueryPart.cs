
namespace PersistanceMap.QueryParts
{
    public interface IQueryPart
    {
        OperationType OperationType { get; }

        string Compile();
    }
}
