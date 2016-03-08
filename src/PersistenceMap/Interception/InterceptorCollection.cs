using System;
using System.Collections.Generic;
using System.Linq;

namespace PersistenceMap.Interception
{
    public class InterceptorCollection
    {
        private readonly List<InterceptorItem> _interceptors = new List<InterceptorItem>();

        public IInterceptor Add<T>(IInterceptor interceptor)
        {
            _interceptors.Add(new InterceptorItem(typeof(T), interceptor));

            return interceptor;
        }

        public IInterceptor Add<T>(IInterceptor<T> interceptor)
        {
            _interceptors.Add(new InterceptorItem(typeof(T), interceptor));

            return interceptor;
        }

        public IInterceptor GetInterceptor<T>()
        {
            var item = _interceptors.FirstOrDefault(i => i.Key == typeof(T));
            return item != null ? item.Interceptor : null;
        }

        public IEnumerable<IInterceptor> GetInterceptors<T>()
        {
            return _interceptors.Where(i => i.Key == typeof(T)).Select(i => i.Interceptor);
        }

        public IEnumerable<IInterceptor> GetInterceptors(Type type)
        {
            return _interceptors.Where(i => i.Key == type).Select(i => i.Interceptor);
        }

        /// <summary>
        /// Removes all interceptors associated to a given type
        /// </summary>
        /// <typeparam name="T">The key type</typeparam>
        public void Remove<T>()
        {
            var items = _interceptors.Where(i => i.Key == typeof(T)).ToList();
            foreach (var item in items)
            {
                _interceptors.Remove(item);
            }
        }
    }
}
