using System;
using System.Collections.Generic;
using PersistenceMap.QueryBuilder;

namespace PersistenceMap.Interception
{
    public class InterceptionHandler
    {
        private readonly IEnumerable<IInterceptor> _interceptors;
        private readonly IDatabaseContext _context;

        public InterceptionHandler(InterceptorCollection collection, Type type, IDatabaseContext context)
        {
            _interceptors = collection.GetInterceptors(type);
            _context = context;
        }

        public void HandleBeforeCompile(IQueryPartsContainer container)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.VisitBeforeCompile(container);
            }
        }

        public void HandleBeforeExecute(CompiledQuery query)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.VisitBeforeExecute(query, _context);
            }
        }
    }

    public class InterceptionHandler<T> : InterceptionHandler
    {
        public InterceptionHandler(InterceptorCollection collection, IDatabaseContext context)
            : base(collection, typeof(T), context)
        {
        }
    }
}
