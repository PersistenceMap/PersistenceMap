using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap
{
    public interface IInterceptor
    {
        void BeforeExecute(CompiledQuery query);

        IEnumerable<T> Execute<T>(CompiledQuery query);
    }

    public interface IInterceptor<T> : IInterceptor
    {
        IInterceptor<T> BeforeExecute(Expression<Action<CompiledQuery>> query);

        IInterceptor<T> Execute(Expression<Func<CompiledQuery, IEnumerable<T>>> query);

        IEnumerable<T> Execute<T>(CompiledQuery query);
    }

    public class Interceptor<T> : IInterceptor<T>
    {
        private Expression<Action<CompiledQuery>> _beforeExecute;
        private Expression<Func<CompiledQuery, IEnumerable<T>>> _execute;
        
        public IInterceptor<T> BeforeExecute(Expression<Action<CompiledQuery>> query)
        {
            _beforeExecute = query;

            return this;
        }

        public IInterceptor<T> Execute(Expression<Func<CompiledQuery, IEnumerable<T>>> query)
        {
            _execute = query;
            
            return this;
        }

        public void BeforeExecute(CompiledQuery query)
        {
            if (_beforeExecute == null)
                return;

            _beforeExecute.Compile().Invoke(query);
        }

        public IEnumerable<T> Execute<T>(CompiledQuery query)
        {
            if (_execute == null)
                return null;

            // the problem is that this is not the same T as in the Class!!!!
            return _execute.Compile().Invoke(query);
        }
    }

    public class InterceptorCollection
    {
        readonly List<InterceptorItem> _interceptors = new List<InterceptorItem>();

        public void Add(Type key, IInterceptor interceptor)
        {
            _interceptors.Add(new InterceptorItem(key, interceptor));
        }

        public IInterceptor GetInterceptor<T>()
        {
            var item = _interceptors.FirstOrDefault(i => i.Key == typeof(T));
            return item != null ? item.Interceptor : null;
        }
    }

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
