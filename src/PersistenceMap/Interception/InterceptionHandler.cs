using System;
using System.Collections.Generic;
using PersistenceMap.QueryBuilder;

namespace PersistenceMap.Interception
{
    public class InterceptionHandler
    {
        private readonly IEnumerable<IInterceptor> _interceptors;
        private readonly IDatabaseContext _context;

        /// <summary>
        /// Creates a InterceptionHandler for intercepting BeforeCompile Interceptors
        /// </summary>
        /// <param name="collection">The interceptors</param>
        /// <param name="type">The type for interception</param>
        public InterceptionHandler(InterceptorCollection collection, Type type)
            : this(collection, type, context: null)
        {
        }

        /// <summary>
        /// Creates a InterceptionHandler for Visiting Interceptors
        /// </summary>
        /// <param name="collection">The interceptors</param>
        /// <param name="type">The type for interception</param>
        /// <param name="context">The database context</param>
        public InterceptionHandler(InterceptorCollection collection, Type type, IDatabaseContext context)
        {
            _interceptors = collection.GetInterceptors(type);
            _context = context;
        }

        /// <summary>
        /// Handle the BeforeCompile Interception Visitors
        /// </summary>
        /// <param name="container">The queryparts</param>
        public void HandleBeforeCompile(IQueryPartsContainer container)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.VisitBeforeCompile(container);
            }
        }

        /// <summary>
        /// Handle the BeforeExecute Interception Visitors
        /// </summary>
        /// <param name="query">The query</param>
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
        /// <summary>
        /// Creates a InterceptionHandler for Visiting Interceptors
        /// </summary>
        /// <param name="collection">The interceptors</param>
        /// <param name="context">The database context</param>
        public InterceptionHandler(InterceptorCollection collection, IDatabaseContext context)
            : base(collection, typeof(T), context)
        {
        }
    }
}
