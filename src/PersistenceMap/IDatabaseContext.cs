using System;
using System.Collections.Generic;
using PersistenceMap.Diagnostics;

namespace PersistenceMap
{
    /// <summary>
    /// The context that is needed to connect to a database
    /// </summary>
    public interface IDatabaseContext : IDisposable
    {
        /// <summary>
        /// Provides a connection to a specific RDBMS
        /// </summary>
        IConnectionProvider ConnectionProvider { get; }

        /// <summary>
        /// Commit the queries contained in the commandstore
        /// </summary>
        void Commit();

        /// <summary>
        /// Add a query to the commandstore
        /// </summary>
        /// <param name="command"></param>
        void AddQuery(IQueryCommand command);

        /// <summary>
        /// Gets the settings of the context
        /// </summary>
        ISettings Settings { get; }

        /// <summary>
        /// The commandstore containing all queries that have not been executed
        /// </summary>
        IEnumerable<IQueryCommand> QueryStore { get; }

        InterceptorCollection Interceptors { get; }

        /// <summary>
        /// The kernel providing the execution of the query and mapping of the data
        /// </summary>
        QueryKernel Kernel { get; }
    }
}
