using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;

namespace PersistanceMap
{
    public interface IInterceptor
    {
    }

    public interface IInterceptor<T> : IInterceptor
    {
        IInterceptor<T> BeforeExecute(Action<CompiledQuery> query);

        IInterceptor<T> Execute(Func<CompiledQuery, IEnumerable<T>> query);
    }

    public interface IInterceptionExecution<T> : IInterceptor
    {
        void OnBeforeExecute(CompiledQuery query);

        IEnumerable<T> OnExecute(CompiledQuery query);
    }
}
