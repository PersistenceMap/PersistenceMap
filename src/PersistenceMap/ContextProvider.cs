using PersistenceMap.Interception;
using System;

namespace PersistenceMap
{
    /// <summary>
    /// Base class for the Contextprovider
    /// Implementations have to be specific for the desired SQL Provider
    /// </summary>
    public class ContextProvider : IContextProvider
    {
        private readonly InterceptorCollection _interceptors = new InterceptorCollection();

        /// <summary>
        /// Base class for the Contextprovider.
        /// Implementations have to be specific for the desired SQL Provider
        /// </summary>
        /// <param name="connectionProvider">The connectionprovider for the desired SQL Provider</param>
        public ContextProvider(IConnectionProvider connectionProvider)
        {
            if (connectionProvider.QueryCompiler == null)
            {
                throw new InvalidOperationException("The ConnectionProvider does not contain a QueryCompiler.");
            }

            ConnectionProvider = connectionProvider;
            Settings = new Settings();
        }

        /// <summary>
        /// The settings for the context
        /// </summary>
        public Settings Settings { get; }

        /// <summary>
        /// The connection to a SqlCe database
        /// </summary>
        public IConnectionProvider ConnectionProvider { get; set; }

        /// <summary>
        /// Gets the interceptorcollection
        /// </summary>
        public InterceptorCollection Interceptors =>  _interceptors;

        /// <summary>
        /// Add a interceptor to the executionmodel
        /// </summary>
        /// <typeparam name="T">The type that will be executed</typeparam>
        /// <returns>A instance of a interceptor</returns>
        public IInterceptionContext<T> Interceptor<T>()
        {
            var context = new InterceptionContext<T>(Interceptors);
            return context;
        }

        public IInterceptionContext<T> Interceptor<T>(Func<T> anonymObject)
        {
            var context = new InterceptionContext<T>(Interceptors);
            return context;
        }

        #region IDisposeable Implementation

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        internal bool IsDisposed { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        public virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing && !IsDisposed)
                {
                    IsDisposed = true;
                }
            }
        }
        
        #endregion
    }
}
