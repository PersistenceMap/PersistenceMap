
namespace PersistanceMap.QueryParts
{
    public interface IEntityQueryPart : IQueryPart
    {
        string Entity { get; }

        string EntityAlias { get; set; }
    }
}
