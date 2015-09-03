using System;

namespace PersistanceMap
{
    public class ContextProvider : IContextProvider
    {
        private readonly InterceptorCollection _interceptors = new InterceptorCollection();

        public ContextProvider(IConnectionProvider connectionProvider)
        {
            ConnectionProvider = connectionProvider;
            Settings = new Settings();
        }

        /// <summary>
        /// The settings for the context
        /// </summary>
        public Settings Settings { get; private set; }

        /// <summary>
        /// The connection to a Sql Server database
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
