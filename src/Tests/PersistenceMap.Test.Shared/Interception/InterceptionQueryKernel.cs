using PersistenceMap.QueryBuilder;
using System;
using System.Collections.Generic;

namespace PersistenceMap.Interception
{
    internal class InterceptionQueryKernel : QueryKernel
    {
        private readonly Dictionary<Type, EnumerableDataReader> _datareaders;

        public InterceptionQueryKernel(IDatabaseContext context)
            : base(context.ConnectionProvider, context.Settings)
        {
            _datareaders = new Dictionary<Type, EnumerableDataReader>();
            var kernel = context.Kernel as InterceptionQueryKernel;
            if (kernel != null)
            {
                foreach (var reader in kernel.DataReaders)
                {
                    _datareaders.Add(reader.Key, reader.Value);
                }
            }
        }

        internal Dictionary<Type, EnumerableDataReader> DataReaders => _datareaders;

        public void AddDataReader<T>(EnumerableDataReader dataReader)
        {
            _datareaders.Add(typeof(T), dataReader);
        }

        public override IEnumerable<T> Execute<T>(CompiledQuery compiledQuery)
        {
            var provider = ConnectionProvider;

            if (_datareaders.ContainsKey(typeof(T)))
            {
                var reader = _datareaders[typeof(T)];
                ConnectionProvider = new InterceptionConnectionProvider(provider.QueryCompiler, reader);
            }

            var items = base.Execute<T>(compiledQuery);

            ConnectionProvider = provider;

            return items;
        }
    }
}
