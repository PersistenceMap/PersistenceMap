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

        /// <summary>
        /// Compile IQueryPartsMap to a QueryString
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryParts"></param>
        /// <returns></returns>
        CompiledQuery Compile<T>(IQueryPartsMap queryParts);
    }
}
