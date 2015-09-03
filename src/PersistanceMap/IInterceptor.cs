using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface IInterceptorBase
    {
    }

    public interface IInterceptor<T> : IInterceptorBase
    {
        IInterceptor<T> BeforeExecute(Action<CompiledQuery> query);

        IInterceptor<T> Execute(Func<CompiledQuery, IEnumerable<T>> query);

        void BeforeExecute(CompiledQuery query);

        IEnumerable<T> Execute(CompiledQuery query);
    }
}
