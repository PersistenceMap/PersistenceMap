using System;
using System.Collections.Generic;
using PersistenceMap.QueryBuilder;

namespace PersistenceMap.Interception
{
    public class CompileInterceptor<T> : IInterceptor, IInterceptor<T>
    {
        private readonly Action<CompiledQuery> _beforeExecute;
        private readonly Action<IQueryPartsContainer> _beforeCompile;

        public CompileInterceptor(Action<CompiledQuery> beforeExecute)
        {
            _beforeExecute = beforeExecute;
        }

        public CompileInterceptor(Action<IQueryPartsContainer> beforeCompile)
        {
            _beforeCompile = beforeCompile;
        }
        
        public void VisitBeforeExecute(CompiledQuery query, IDatabaseContext context)
        {
            if (_beforeExecute == null)
            {
                return;
            }

            _beforeExecute.Invoke(query);
        }

        public IEnumerable<TRes> VisitOnExecute<TRes>(CompiledQuery query)
        {
            return null;
        }

        public bool VisitOnExecute(CompiledQuery query)
        {
            return false;
        }

        public void VisitBeforeCompile(IQueryPartsContainer container)
        {
            if (_beforeCompile == null)
            {
                return;
            }

            _beforeCompile(container);
        }
    }
}
