
namespace PersistanceMap.QueryBuilder
{
    public interface IFieldQueryMap : IEntityQueryPart, IQueryPart
    {
        string Field { get; }

        string FieldAlias { get; }
    }
}
