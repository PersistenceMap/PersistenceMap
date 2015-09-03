using System;
using System.Collections.Generic;
using System.Linq;

namespace PersistanceMap
{
    public class InterceptorCollection
    {
        readonly List<InterceptorItem> _interceptors = new List<InterceptorItem>();

        public void Add(Type key, IInterceptorBase interceptor)
        {
            _interceptors.Add(new InterceptorItem(key, interceptor));
        }

        public IInterceptor<T> GetInterceptor<T>()
        {
            var item = _interceptors.FirstOrDefault(i => i.Key == typeof(T));
            return item != null ? item.Interceptor as IInterceptor<T> : null;
        }
    }
}
