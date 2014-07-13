using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap
{
    public interface IDbContext : IDisposable
    {
        IContextProvider ContextProvider { get; }

        IEnumerable<T> Execute<T>(CompiledQuery compiledQuery);
    }
}
