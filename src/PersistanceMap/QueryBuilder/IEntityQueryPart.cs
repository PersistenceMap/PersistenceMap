
namespace PersistanceMap.QueryBuilder
{
    public interface IEntityQueryPart : IQueryPart
    {
        string Entity { get; }

        string EntityAlias { get; set; }
    }
}
