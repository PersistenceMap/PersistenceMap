using System.Collections.Generic;
using PersistenceMap.QueryBuilder;

namespace PersistenceMap.Interception
{
    /// <summary>
    /// Defines an interceptor
    /// </summary>
    public interface IInterceptor
    {
        void VisitBeforeExecute(CompiledQuery query);

        IEnumerable<T> VisitOnExecute<T>(CompiledQuery query);

        bool VisitOnExecute(CompiledQuery query);

        void VisitBeforeCompile(IQueryPartsContainer container);
    }

    public interface IInterceptor<T> : IInterceptor
    {
    }
}
