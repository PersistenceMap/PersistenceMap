using System;
using System.Collections.Generic;

namespace PersistanceMap
{
    public interface IDatabaseContext : IDisposable
    {
        //IContextProvider ContextProvider { get; }

        IConnectionProvider ConnectionProvider { get; }

        void Commit();

        void AddQuery(IQueryCommand command);

        IEnumerable<IQueryCommand> QueryCommandStore { get; }

        QueryKernel Kernel { get; }
    }
}
