using System;
using System.Collections.Generic;
using PersistanceMap.Tracing;

namespace PersistanceMap
{
    /// <summary>
    /// The context that is needed to connect to a database
    /// </summary>
    public interface IDatabaseContext : IDisposable
    {
        /// <summary>
        /// Gets the Loggerfactory for logging
        /// </summary>
        ILoggerFactory LoggerFactory { get; }

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
        /// The commandstore containing all queries that have not been executed
        /// </summary>
        IEnumerable<IQueryCommand> QueryCommandStore { get; }

        /// <summary>
        /// The kernel providing the execution of the query and mapping of the data
        /// </summary>
        QueryKernel Kernel { get; }
    }
}
