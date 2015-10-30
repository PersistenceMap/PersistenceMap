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
        IConnectionProvider ConnectionProvider { get; }

        /// <summary>
        /// Add a interceptor to the executionmodel
        /// </summary>
        /// <typeparam name="T">The type that will be executed</typeparam>
        /// <returns>A instance of a interceptor</returns>
        IInterceptor<T> Interceptor<T>();
    }
}
