using PersistenceMap.QueryBuilder;
using System;
using System.Collections.Generic;

namespace PersistenceMap.Interception
{
    public class InterceptionContext<T> : IInterceptionContext<T>
    {
        private readonly InterceptorCollection _interceptors;
        private readonly IInterceptionBuilder<T> _interceptor;

        public InterceptionContext(InterceptorCollection interceptors, IInterceptionBuilder<T> interceptor)
        {
            _interceptors = interceptors;
            _interceptor = interceptor;
        }

        public IInterceptionBuilder<T> BeforeCompile(Action<IQueryPartsContainer> container)
        {
            _interceptor.BeforeCompile(container);

            return this;
        }

        public IInterceptionBuilder<T> BeforeExecute(Action<CompiledQuery> query)
        {
            _interceptor.BeforeExecute(query);

            return this;
        }

        public IInterceptionBuilder<T> AsExecute(Func<CompiledQuery, IEnumerable<T>> query)
        {
            _interceptor.AsExecute(query);

            return this;
        }

        public IInterceptionBuilder<T> AsExecute(Action<CompiledQuery> query)
        {
            _interceptor.AsExecute(query);

            return this;
        }
    }
}
