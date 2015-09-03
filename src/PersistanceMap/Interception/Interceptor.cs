using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public class Interceptor<T> : IInterceptor<T>
    {
        private Action<CompiledQuery> _beforeExecute;
        private Expression<Func<CompiledQuery, IEnumerable<T>>> _execute;

        public IInterceptor<T> BeforeExecute(Action<CompiledQuery> query)
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

            _beforeExecute.Invoke(query);
        }

        public IEnumerable<T> Execute(CompiledQuery query)
        {
            if (_execute == null)
                return null;

            // the problem is that this is not the same T as in the Class!!!!
            return _execute.Compile().Invoke(query);
        }
    }
}
