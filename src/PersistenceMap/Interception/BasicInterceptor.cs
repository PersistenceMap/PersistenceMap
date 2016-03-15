using System;
using PersistenceMap.QueryBuilder;

namespace PersistenceMap.Interception
{
    internal class BasicInterceptor<T> : IInterceptor, IInterceptor<T>
    {
        private readonly Action<CompiledQuery> _beforeExecute;
        private readonly Action<IQueryPartsContainer> _beforeCompile;

        public BasicInterceptor(Action<CompiledQuery> beforeExecute)
        {
            _beforeExecute = beforeExecute;
        }

        public BasicInterceptor(Action<IQueryPartsContainer> beforeCompile)
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
