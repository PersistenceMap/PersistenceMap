using System.Collections.Generic;

namespace PersistenceMap.Interception
{
    public static class InterceptorExtensions
    {
        public static IInterceptionContext<T> Returns<T>(this IInterceptionContext<T> interceptionContext, IEnumerable<T> list)
        {
            var interceptor = new MockInterceptor<T>(list);
            interceptionContext.Interceptors.Add(interceptor);

            return interceptionContext;
        }
    }
}
