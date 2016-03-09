using System;
using System.Collections.Generic;
using System.Linq;
using PersistenceMap.QueryBuilder;

namespace PersistenceMap.Interception
{
    public class ExecutionInterceptor<T> : IInterceptor, IInterceptor<T>
    {
        private readonly Func<CompiledQuery, IEnumerable<T>> _execute;
        private readonly Action<CompiledQuery> _executeNonQuery;

        public ExecutionInterceptor(Func<CompiledQuery, IEnumerable<T>> execute)
        {
            _execute = execute;
        }

        public ExecutionInterceptor(Action<CompiledQuery> executeNonQuery)
        {
            _executeNonQuery = executeNonQuery;
        }
        
        public void VisitBeforeExecute(CompiledQuery query, IDatabaseContext context)
        {
        }

        public IEnumerable<TRes> VisitOnExecute<TRes>(CompiledQuery query)
        {
            if (_execute == null)
            {
                return null;
            }

            // the problem is that this is not the same T as in the Class!!!!
            return _execute.Invoke(query).Cast<TRes>();
        }

        public bool VisitOnExecute(CompiledQuery query)
        {
            if (_executeNonQuery == null)
            {
                return false;
            }

            _executeNonQuery(query);

            return true;
        }

        public void VisitBeforeCompile(IQueryPartsContainer container)
        {
        }
    }
}
