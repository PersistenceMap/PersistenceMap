using System;

namespace PersistenceMap
{
    /// <summary>
    /// Base class for the Contextprovider
    /// Implementations have to be specific for the desired SQL Provider
    /// </summary>
    public abstract class ContextProvider : IContextProvider
    {
        private readonly InterceptorCollection _interceptors = new InterceptorCollection();

        public ContextProvider(IConnectionProvider connectionProvider)
        {
            ConnectionProvider = connectionProvider;
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
        public IInterceptor<T> Interceptor<T>()
        {
            return _interceptors.Add(new Interceptor<T>());
        }

        public IInterceptor<T> Interceptor<T>(Func<T> anonymObject)
        {
            return _interceptors.Add(new Interceptor<T>());
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
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Releases resources before the object is reclaimed by garbage collection.
        /// </summary>
        ~ContextProvider()
        {
            Dispose(false);
        }

        #endregion
    }
}
