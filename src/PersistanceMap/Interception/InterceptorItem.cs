using System;

namespace PersistanceMap
{
    internal class InterceptorItem
    {
        public InterceptorItem(Type key, IInterceptor interceptor)
        {
            Key = key;
            Interceptor = interceptor;
        }

        public Type Key { get; private set; }

        public IInterceptor Interceptor { get; private set; }
    }
}
