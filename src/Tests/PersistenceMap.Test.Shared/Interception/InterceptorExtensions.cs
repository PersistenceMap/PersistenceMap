using System;
using System.Collections.Generic;

namespace PersistenceMap.Interception
{
    public static class InterceptorExtensions
    {
        public static IInterceptionContext<T> Returns<T>(this IInterceptionContext<T> interceptionContext, IEnumerable<T> list)
        {
            var interceptor = new DataReaderInterceptor<T>(list);
            interceptionContext.Interceptors.Add(interceptor);

            return interceptionContext;
        }

        public static IInterceptionContext<T> Returns<T>(this IInterceptionContext<T> interceptionContext, Func<IEnumerable<T>> list)
        {
            var interceptor = new DataReaderInterceptor<T>(list.Invoke());
            interceptionContext.Interceptors.Add(interceptor);

            return interceptionContext;
        }

        public static IInterceptionBuilder<T> Returns<T>(this IInterceptionBuilder<T> interceptionBuilder, IEnumerable<T> list)
        {
            var context = interceptionBuilder as IInterceptionContext<T>;
            if (context == null)
            {
                return interceptionBuilder;
            }

            var interceptor = new DataReaderInterceptor<T>(list);
            context.Interceptors.Add(interceptor);

            return interceptionBuilder;
        }

        public static IInterceptionBuilder<T> Returns<T>(this IInterceptionBuilder<T> interceptionBuilder, Func<IEnumerable<T>> list)
        {
            var context = interceptionBuilder as IInterceptionContext<T>;
            if (context == null)
            {
                return interceptionBuilder;
            }

            var interceptor = new DataReaderInterceptor<T>(list.Invoke());
            context.Interceptors.Add(interceptor);

            return interceptionBuilder;
        }
    }
}
