using System.Collections.Generic;
using PersistenceMap.QueryBuilder;

namespace PersistenceMap.Interception
{
    /// <summary>
    /// Defines an interceptor
    /// </summary>
    public interface IInterceptor
    {
        void VisitBeforeExecute(CompiledQuery query, IDatabaseContext context);
        
        void VisitBeforeCompile(IQueryPartsContainer container);
    }

    public interface IInterceptor<T> : IInterceptor
    {
    }
}
