using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    /// <summary>
    /// Compiles IQueryPartsContainer to a QueryString
    /// </summary>
    public interface IQueryCompiler
    {
        /// <summary>
        /// Compile IQueryPartsContainer to a QueryString
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        CompiledQuery Compile(IQueryPartsContainer container);
    }
}
