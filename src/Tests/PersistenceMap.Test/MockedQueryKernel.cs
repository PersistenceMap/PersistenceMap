using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersistenceMap.QueryBuilder;

namespace PersistenceMap.Test
{
    public class MockedQueryKernel : QueryKernel
    {
        public MockedQueryKernel(IConnectionProvider provider, ISettings settings, InterceptorCollection interceptors)
            : base(provider, settings, interceptors)
        {
        }

        public override IEnumerable<T> Execute<T>(CompiledQuery compiledQuery)
        {
            return base.Execute<T>(compiledQuery);
        }

        public override void Execute(CompiledQuery compiledQuery)
        {
            base.Execute(compiledQuery);
        }

        public override void Execute(CompiledQuery compiledQuery, params Action<IDataReaderContext>[] expressions)
        {
            base.Execute(compiledQuery, expressions);
        }
    }
}
