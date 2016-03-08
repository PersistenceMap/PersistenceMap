using PersistenceMap.Interception;
using PersistenceMap.QueryBuilder;

namespace PersistenceMap
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
        /// <param name="interceptorColelction"></param>
        /// <returns></returns>
        CompiledQuery Compile(IQueryPartsContainer container, InterceptorCollection interceptorColelction);
    }
}
