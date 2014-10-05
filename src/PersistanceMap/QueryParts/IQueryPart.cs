
namespace PersistanceMap.QueryParts
{
    public interface IQueryPart
    {
        string ID { get; set; }

        OperationType OperationType { get; }

        string Compile();
    }
}
