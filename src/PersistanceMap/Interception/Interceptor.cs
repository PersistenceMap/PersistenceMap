using System;
using System.Collections.Generic;
using System.Linq;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public class Interceptor<T> : IInterceptor<T>, IInterceptorExecution
    {
        private Action<CompiledQuery> _beforeExecute;
        private Func<CompiledQuery, IEnumerable<T>> _execute;
        private Action<CompiledQuery> _executeNonQuery;
        private Action<IQueryPartsContainer> _beforeCompile;

        public IInterceptor<T> BeforeCompile(Action<IQueryPartsContainer> container)
        {
            _beforeCompile = container;

            return this;
        }

        public IInterceptor<T> BeforeExecute(Action<CompiledQuery> query)
        {
            _beforeExecute = query;

            return this;
        }

        public IInterceptor<T> AsExecute(Func<CompiledQuery, IEnumerable<T>> query)
        {
            _execute = query;

            return this;
        }

        public IInterceptor<T> AsExecute(Action<CompiledQuery> query)
        {
            _executeNonQuery = query;

            return this;
        }

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
    }
}
