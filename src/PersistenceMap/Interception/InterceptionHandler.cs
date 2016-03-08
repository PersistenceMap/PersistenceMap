using System;
using System.Collections.Generic;
using PersistenceMap.QueryBuilder;

namespace PersistenceMap.Interception
{
    internal class InterceptionHandler<T>
    {
        private readonly IEnumerable<IInterceptor> _interceptors;

        public InterceptionHandler(InterceptorCollection collection)
        {
            _interceptors = collection.GetInterceptors<T>();
        }

        public void BeforeExecute(CompiledQuery query)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.ExecuteBeforeExecute(query);
            }
        }

        public IEnumerable<T> Execute(CompiledQuery query)
        {
            foreach (var interceptor in _interceptors)
            {
                var items = interceptor.Execute<T>(query);
                if (items != null)
                {
                    return items;
                }
            }

            return null;
        }
    }

    internal class InterceptionHandler
    {
        private readonly IEnumerable<IInterceptor> _interceptors;

        public InterceptionHandler(InterceptorCollection collection, Type type)
        {
            _interceptors = collection.GetInterceptors(type);
        }

        public void ExecuteBeforeCompile(IQueryPartsContainer container)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.ExecuteBeforeCompile(container);
            }
        }

        public void BeforeExecute(CompiledQuery query)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.ExecuteBeforeExecute(query);
            }
        }

        public bool Execute(CompiledQuery query)
        {
            foreach (var interceptor in _interceptors)
            {
                if(interceptor.Execute(query))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
