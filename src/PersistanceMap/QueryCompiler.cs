using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    /// <summary>
    /// Compiels the QueryParts to a sql string
    /// </summary>
    public class QueryCompiler : IQueryCompiler
    {
        /// <summary>
        /// Compile IQueryPartsMap to a QueryString
        /// </summary>
        /// <param name="queryParts"></param>
        /// <returns></returns>
        public virtual CompiledQuery Compile(IQueryPartsMap queryParts)
        {
            return queryParts.Compile();
        }
    }
}
