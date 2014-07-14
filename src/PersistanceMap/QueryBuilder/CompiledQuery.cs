
namespace PersistanceMap.QueryBuilder
{
    public class CompiledQuery
    {
        public string QueryString { get; internal set; }

        public IQueryPartsMap QueryParts { get; internal set; }
    }
}
