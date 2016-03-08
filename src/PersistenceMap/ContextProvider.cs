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
            ConnectionProvider = connectionProvider;
            Settings = new Settings();
        }

        /// <summary>
        /// The settings for the context
        /// </summary>
        public Settings Settings { get; protected set; }

        /// <summary>
        /// The connection to a SqlCe database
        /// </summary>
        public IConnectionProvider ConnectionProvider { get; protected set; }

        /// <summary>
        /// Gets the interceptorcollection
        /// </summary>
        public InterceptorCollection Interceptors
        {
            get
            {
                return _interceptors;
            }
        }

        /// <summary>
        /// Add a interceptor to the executionmodel
        /// </summary>
        /// <typeparam name="T">The type that will be executed</typeparam>
        /// <returns>A instance of a interceptor</returns>
        public IInterceptionContext<T> Interceptor<T>()
        {
            var interceptor = new Interceptor<T>();
            _interceptors.Add(interceptor);
            var context = new InterceptionContext<T>(Interceptors, interceptor);
            return context;
        }

        public IInterceptionContext<T> Interceptor<T>(Func<T> anonymObject)
        {
            var interceptor = new Interceptor<T>();
            _interceptors.Add(interceptor);
            var context = new InterceptionContext<T>(Interceptors, interceptor);
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
