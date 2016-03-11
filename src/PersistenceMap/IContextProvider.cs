using System;

namespace PersistenceMap
{
    /// <summary>
    /// Provides a database context
    /// </summary>
    public interface IContextProvider : IDisposable
    {
        /// <summary>
        /// The connection to a RDBMS
        /// </summary>
        IConnectionProvider ConnectionProvider { get; set; }
    }
}
