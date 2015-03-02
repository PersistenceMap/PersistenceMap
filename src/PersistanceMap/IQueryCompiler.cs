using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    /// <summary>
    /// Compiles IQueryPartsMap to a QueryString
    /// </summary>
    public interface IQueryCompiler
    {
        /// <summary>
        /// Compile IQueryPartsMap to a QueryString
        /// </summary>
        /// <param name="queryParts"></param>
        /// <returns></returns>
        CompiledQuery Compile(IQueryPartsMap queryParts);
    }
}
