
namespace PersistenceMap.QueryBuilder
{
    public class CompiledQuery
    {
        public string QueryString { get; set; }

        public IQueryPartsContainer QueryParts { get; set; }
    }
}
