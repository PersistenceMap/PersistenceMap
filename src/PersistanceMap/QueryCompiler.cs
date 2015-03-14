using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    /// <summary>
    /// Compiels the QueryParts to a sql string
    /// </summary>
    public class QueryCompiler : IQueryCompiler
    {
        /// <summary>
        /// Compile IQueryPartsContainer to a QueryString
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public virtual CompiledQuery Compile(IQueryPartsContainer container)
        {
            return container.Compile();
        }
    }
}
