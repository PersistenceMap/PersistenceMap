using System;
using System.Collections.Generic;
using System.Linq;

namespace PersistanceMap
{
    public class InterceptorCollection
    {
        private readonly List<InterceptorItem> _interceptors = new List<InterceptorItem>();

        public IInterceptor<T> Add<T>(IInterceptor<T> interceptor)
        {
            var item = _interceptors.FirstOrDefault(i => i.Key == typeof(T));
            if (item != null)
            {
                var existing = item.Interceptor as IInterceptor<T>;
                if (existing != null)
                {
                    return existing;
                }
            }

            _interceptors.Add(new InterceptorItem(typeof(T), interceptor));

            return interceptor;
        }

        public IInterceptor<T> GetInterceptor<T>()
        {
            var item = _interceptors.FirstOrDefault(i => i.Key == typeof(T));
            return item != null ? item.Interceptor as IInterceptor<T> : null;
        }

        public void Remove<T>()
        {
            var item = _interceptors.FirstOrDefault(i => i.Key == typeof(T));
            if (item != null)
            {
                _interceptors.Remove(item);
            }
        }
    }
}
