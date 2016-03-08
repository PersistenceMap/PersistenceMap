using System;
using System.Collections.Generic;
using System.Linq;
using PersistenceMap.QueryBuilder;

namespace PersistenceMap.Interception
{
    public class Interceptor<T> : IInterceptor, IInterceptor<T>, IInterceptionBuilder<T>
    {
        private Action<CompiledQuery> _beforeExecute;
        private Func<CompiledQuery, IEnumerable<T>> _execute;
        private Action<CompiledQuery> _executeNonQuery;
        private Action<IQueryPartsContainer> _beforeCompile;
        
        public void ExecuteBeforeExecute(CompiledQuery query)
        {
            if (_beforeExecute == null)
            {
                return;
            }

            _beforeExecute.Invoke(query);
        }

        public IEnumerable<TRes> Execute<TRes>(CompiledQuery query)
        {
            if (_execute == null)
            {
                return null;
            }

            // the problem is that this is not the same T as in the Class!!!!
            return _execute.Invoke(query).Cast<TRes>();
        }

        public bool Execute(CompiledQuery query)
        {
            if (_executeNonQuery == null)
            {
                return false;
            }

            _executeNonQuery(query);

            return true;
        }

        public void ExecuteBeforeCompile(IQueryPartsContainer container)
        {
            if (_beforeCompile == null)
            {
                return;
            }

            _beforeCompile(container);
        }

        public Interceptor<T> BeforeCompile(Action<IQueryPartsContainer> container)
        {
            _beforeCompile = container;

            return this;
        }

        public Interceptor<T> BeforeExecute(Action<CompiledQuery> query)
        {
            _beforeExecute = query;

            return this;
        }

        public Interceptor<T> AsExecute(Func<CompiledQuery, IEnumerable<T>> query)
        {
            _execute = query;

            return this;
        }

        public Interceptor<T> AsExecute(Action<CompiledQuery> query)
        {
            _executeNonQuery = query;

            return this;
        }

        IInterceptionBuilder<T> IInterceptionBuilder<T>.BeforeCompile(Action<IQueryPartsContainer> container)
        {
            return BeforeCompile(container);
        }

        IInterceptionBuilder<T> IInterceptionBuilder<T>.BeforeExecute(Action<CompiledQuery> query)
        {
            return BeforeExecute(query);
        }

        IInterceptionBuilder<T> IInterceptionBuilder<T>.AsExecute(Func<CompiledQuery, IEnumerable<T>> query)
        {
            return AsExecute(query);
        }

        IInterceptionBuilder<T> IInterceptionBuilder<T>.AsExecute(Action<CompiledQuery> query)
        {
            return AsExecute(query);
        }
    }
}
