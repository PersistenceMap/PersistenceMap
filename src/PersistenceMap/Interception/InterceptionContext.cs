using PersistenceMap.QueryBuilder;
using System;
using System.Collections.Generic;

namespace PersistenceMap.Interception
{
    public class InterceptionContext<T> : IInterceptionContext<T>
    {
        private readonly InterceptorCollection _interceptors;

        /// <summary>
        /// Gets the InterceptionCollection related to the context
        /// </summary>
        public InterceptorCollection Interceptors => _interceptors;

        public InterceptionContext(InterceptorCollection interceptors)
        {
            _interceptors = interceptors;
        }

        public IInterceptionBuilder<T> BeforeCompile(Action<IQueryPartsContainer> container)
        {
            var interceptor = new CompileInterceptor<T>(container);
            _interceptors.Add(interceptor);

            return this;
        }

        public IInterceptionBuilder<T> BeforeExecute(Action<CompiledQuery> query)
        {
            var interceptor = new CompileInterceptor<T>(query);
            _interceptors.Add(interceptor);

            return this;
        }
        
        public IInterceptionBuilder<T> AsExecute(Action<CompiledQuery> query)
        {
            var interceptor = new ExecutionInterceptor<T>(query);
            _interceptors.Add(interceptor);

            return this;
        }
    }
}
