using PersistenceMap.QueryBuilder;
using System;
using System.Collections.Generic;

namespace PersistenceMap.Interception
{
    internal class DataReaderInterceptor<T> : IInterceptor<T>, IReaderInterceptor
    {
        private readonly Func<IEnumerable<T>> _result;
        private readonly Queue<Result> _additionalResults;

        public DataReaderInterceptor(Func<IEnumerable<T>> result)
        {
            _result = result;
            _additionalResults = new Queue<Result>();
        }

        public void VisitBeforeExecute(CompiledQuery query, IDatabaseContext context)
        {
            var reader = new MockedDataReader<T>(_result.Invoke());
            foreach (var result in _additionalResults)
            {
                reader.AddResult(result);
            }

            var kernel = new MockedQueryKernel(context);
            kernel.AddDataReader<T>(reader);

            context.Kernel = kernel;
        }
        
        public void VisitBeforeCompile(IQueryPartsContainer container)
        {
        }

        public IReaderInterceptor AddResult<TRes>(Func<IEnumerable<TRes>> collection)
        {
            _additionalResults.Enqueue(new Result(collection, typeof(TRes)));

            return this;
        }
    }
}
