using PersistenceMap.QueryBuilder;
using System;
using System.Collections.Generic;

namespace PersistenceMap.Interception
{
    internal class DataReaderInterceptor<T> : IInterceptor<T>
    {
        private readonly Func<IEnumerable<T>> _result;

        public DataReaderInterceptor(Func<IEnumerable<T>> result)
        {
            _result = result;
        }

        public void VisitBeforeExecute(CompiledQuery query, IDatabaseContext context)
        {
            var kernel = new MockedQueryKernel(context);
            kernel.AddDataReader<T>(new MockedDataReader<T>(_result.Invoke()));

            context.Kernel = kernel;
        }
        
        public void VisitBeforeCompile(IQueryPartsContainer container)
        {
        }
    }
}
