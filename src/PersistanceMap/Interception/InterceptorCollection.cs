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
            // TODO: Check if this is correct or if it should be able to add multiple interceptors for same type
            //var item = _interceptors.FirstOrDefault(i => i.Key == typeof(T));
            //if (item != null)
            //{
            //    var existing = item.Interceptor as IInterceptor<T>;
            //    if (existing != null)
            //    {
            //        return existing;
            //    }
            //}

            _interceptors.Add(new InterceptorItem(typeof(T), interceptor));

            return interceptor;
        }

        public IInterceptionExecution<T> GetInterceptor<T>()
        {
            var item = _interceptors.FirstOrDefault(i => i.Key == typeof(T));
            return item != null ? item.Interceptor as IInterceptionExecution<T> : null;
        }

        public IEnumerable<IInterceptionExecution<T>> GetInterceptors<T>()
        {
            return _interceptors.Where(i => i.Key == typeof(T)).Select(i => i.Interceptor as IInterceptionExecution<T>);
        }

        /// <summary>
        /// Removes all interceptors associated to a given type
        /// </summary>
        /// <typeparam name="T">The key type</typeparam>
        public void Remove<T>()
        {
            var items = _interceptors.Where(i => i.Key == typeof(T));
            foreach (var item in items)
            {
                _interceptors.Remove(item);
            }
        }
    }
}
