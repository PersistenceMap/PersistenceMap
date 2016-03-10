using PersistenceMap.QueryBuilder;
using System.Collections.Generic;

namespace PersistenceMap.Interception
{
    internal class MockInterceptor<T> : IInterceptor<T>
    {
        private readonly IEnumerable<T> _result;

        public MockInterceptor(IEnumerable<T> result)
        {
            _result = result;
        }

        public void VisitBeforeExecute(CompiledQuery query, IDatabaseContext context)
        {
            var dataReader = new EnumerableDataReader(_result);
            var kernel = new InterceptionQueryKernel(context);
            kernel.AddDataReader<T>(dataReader);

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
