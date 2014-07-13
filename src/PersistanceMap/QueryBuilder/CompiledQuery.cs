
namespace PersistanceMap.QueryBuilder
{
    public class CompiledQuery
    {
        public string QueryString { get; internal set; }

        public QueryPartsContainer QueryParts { get; internal set; }
    }
}
