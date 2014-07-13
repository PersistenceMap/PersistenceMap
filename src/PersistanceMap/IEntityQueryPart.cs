
namespace PersistanceMap
{
    public interface IEntityQueryPart : IQueryPart
    {
        string Entity { get; }

        string Identifier { get; set; }
    }
}
