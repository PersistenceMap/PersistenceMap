using System;

namespace PersistanceMap
{
    internal class InterceptorItem
    {
        public InterceptorItem(Type key, IInterceptorBase interceptor)
        {
            Key = key;
            Interceptor = interceptor;
        }

        public Type Key { get; private set; }

        public IInterceptorBase Interceptor { get; private set; }
    }
}
