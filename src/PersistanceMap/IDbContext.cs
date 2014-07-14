using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;

namespace PersistanceMap
{
    public interface IDbContext : IDisposable
    {
        IContextProvider ContextProvider { get; }

        IEnumerable<T> Execute<T>(CompiledQuery compiledQuery);

        void Execute(CompiledQuery compiledQuery);
    }
}
