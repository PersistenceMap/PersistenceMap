using System.Collections.Generic;
using PersistenceMap.QueryBuilder;

namespace PersistenceMap.Interception
{
    /// <summary>
    /// Defines an interceptor
    /// </summary>
    public interface IInterceptor
    {
        void ExecuteBeforeExecute(CompiledQuery query);

        IEnumerable<T> Execute<T>(CompiledQuery query);

        bool Execute(CompiledQuery query);

        void ExecuteBeforeCompile(IQueryPartsContainer container);
    }

    public interface IInterceptor<T> : IInterceptor
    {
    }
}
