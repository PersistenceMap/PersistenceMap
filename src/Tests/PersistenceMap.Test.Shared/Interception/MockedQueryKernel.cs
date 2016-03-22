using PersistenceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Data;

namespace PersistenceMap.Interception
{
    internal class MockedQueryKernel : QueryKernel
    {
        private readonly Dictionary<Type, IDataReader> _datareaders;

        public MockedQueryKernel(IDatabaseContext context)
            : base(context.ConnectionProvider, context.Settings)
        {
            _datareaders = new Dictionary<Type, IDataReader>();
            var kernel = context.Kernel as MockedQueryKernel;
            if (kernel != null)
            {
                foreach (var reader in kernel.DataReaders)
                {
                    _datareaders.Add(reader.Key, reader.Value);
                }
            }
        }

        internal Dictionary<Type, IDataReader> DataReaders => _datareaders;

        public void AddDataReader<T>(IDataReader dataReader)
        {
            // only allow one datareader per type
            _datareaders[typeof(T)] = dataReader;
        }

        public override IEnumerable<T> Execute<T>(CompiledQuery compiledQuery)
        {
            var provider = ConnectionProvider;

            if (_datareaders.ContainsKey(typeof(T)))
            {
                var reader = _datareaders[typeof(T)];
                ConnectionProvider = new MockedConnectionProvider(provider.QueryCompiler, reader);
            }

            var items = base.Execute<T>(compiledQuery);

            ConnectionProvider = provider;

            return items;
        }

        public override IEnumerable<ReaderResult> Execute(CompiledQuery query)
        {
            var provider = ConnectionProvider;

            if (_datareaders.ContainsKey(query.QueryParts.AggregateType))
            {
                var reader = _datareaders[query.QueryParts.AggregateType];
                ConnectionProvider = new MockedConnectionProvider(provider.QueryCompiler, reader);
            }

            var items = base.Execute(query);

            ConnectionProvider = provider;

            return items;
        }
    }
}
