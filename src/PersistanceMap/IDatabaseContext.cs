using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;

namespace PersistanceMap
{
    public interface IDatabaseContext : IDisposable
    {
        IContextProvider ContextProvider { get; }

        IEnumerable<T> Execute<T>(CompiledQuery compiledQuery);

        void Execute(CompiledQuery compiledQuery);

        void Execute(CompiledQuery compiledQuery, params Action<IReaderContext>[] expressions);

        IEnumerable<T> Map<T>(IReaderContext reader);
    }
}
