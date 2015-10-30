using System;
using System.Collections.Generic;
using System.Linq;

namespace PersistenceMap
{
    public class InterceptorCollection
    {
        private readonly List<InterceptorItem> _interceptors = new List<InterceptorItem>();

        public IInterceptor<T> Add<T>(IInterceptor<T> interceptor)
        {
            _interceptors.Add(new InterceptorItem(typeof(T), interceptor));

            return interceptor;
        }

        public IInterceptorExecution GetInterceptor<T>()
        {
            var item = _interceptors.FirstOrDefault(i => i.Key == typeof(T));
            return item != null ? item.Interceptor as IInterceptorExecution : null;
        }

        public IEnumerable<IInterceptorExecution> GetInterceptors<T>()
        {
            return _interceptors.Where(i => i.Key == typeof(T)).Select(i => i.Interceptor as IInterceptorExecution);
        }

        public IEnumerable<IInterceptorExecution> GetInterceptors(Type type)
        {
            return _interceptors.Where(i => i.Key == type).Select(i => i.Interceptor as IInterceptorExecution);
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
