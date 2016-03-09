using System;
using System.Collections.Generic;
using PersistenceMap.QueryBuilder;

namespace PersistenceMap.Interception
{
    internal class InterceptionHandler<T>
    {
        private readonly IEnumerable<IInterceptor> _interceptors;
        private readonly IDatabaseContext _context;

        public InterceptionHandler(InterceptorCollection collection, IDatabaseContext context)
        {
            _interceptors = collection.GetInterceptors<T>();
            _context = context;
        }

        public void BeforeExecute(CompiledQuery query)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.VisitBeforeExecute(query, _context);
            }
        }

        public IEnumerable<T> Execute(CompiledQuery query)
        {
            foreach (var interceptor in _interceptors)
            {
                var items = interceptor.VisitOnExecute<T>(query);
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
        private readonly IDatabaseContext _context;

        public InterceptionHandler(InterceptorCollection collection, Type type, IDatabaseContext context)
        {
            _interceptors = collection.GetInterceptors(type);
            _context = context;
        }

        public void ExecuteBeforeCompile(IQueryPartsContainer container)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.VisitBeforeCompile(container);
            }
        }

        public void BeforeExecute(CompiledQuery query)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.VisitBeforeExecute(query, _context);
            }
        }

        public bool Execute(CompiledQuery query)
        {
            foreach (var interceptor in _interceptors)
            {
                if(interceptor.VisitOnExecute(query))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
