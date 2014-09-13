
namespace PersistanceMap.QueryParts
{
    public interface IFieldQueryPart : IEntityQueryPart, IQueryPart
    {
        string Field { get; }

        string FieldAlias { get; }
    }
}
