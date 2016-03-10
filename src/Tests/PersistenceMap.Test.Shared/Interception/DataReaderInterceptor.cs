using PersistenceMap.QueryBuilder;
using System.Collections.Generic;

namespace PersistenceMap.Interception
{
    internal class DataReaderInterceptor<T> : IInterceptor<T>
    {
        private readonly MockedDataReader<T> _dataReader;

        public DataReaderInterceptor(IEnumerable<T> result)
        {
            _dataReader = new MockedDataReader<T>(result);
        }

        public void VisitBeforeExecute(CompiledQuery query, IDatabaseContext context)
        {
            var kernel = new MockedQueryKernel(context);
            kernel.AddDataReader<T>(_dataReader);

            context.Kernel = kernel;
        }

        public IEnumerable<T1> VisitOnExecute<T1>(CompiledQuery query)
        {
            return null;
        }

        public bool VisitOnExecute(CompiledQuery query)
        {
            return false;
        }

        public void VisitBeforeCompile(IQueryPartsContainer container)
        {
        }
    }
}
