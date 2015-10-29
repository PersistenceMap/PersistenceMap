using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;

namespace PersistanceMap
{
    public class Interceptor<T> : IInterceptor<T>, IInterceptionExecution<T>
    {
        private Action<CompiledQuery> _beforeExecute;
        private Func<CompiledQuery, IEnumerable<T>> _execute;
        private Action<IQueryPartsContainer> _beforeCompile;

        public IInterceptor<T> BeforeCompile(Action<IQueryPartsContainer> container)
        {
            System.Diagnostics.Debug.Assert(false);

            _beforeCompile = container;

            return this;
        }

        public IInterceptor<T> BeforeExecute(Action<CompiledQuery> query)
        {
            _beforeExecute = query;

            return this;
        }

        public IInterceptor<T> Execute(Func<CompiledQuery, IEnumerable<T>> query)
        {
            _execute = query;

            return this;
        }

        public void OnBeforeExecute(CompiledQuery query)
        {
            if (_beforeExecute == null)
                return;

            _beforeExecute.Invoke(query);
        }

        public IEnumerable<T> OnExecute(CompiledQuery query)
        {
            if (_execute == null)
                return null;

            // the problem is that this is not the same T as in the Class!!!!
            return _execute.Invoke(query);
        }
    }
}
