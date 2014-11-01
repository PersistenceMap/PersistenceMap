using PersistanceMap.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersistanceMap
{
    /// <summary>
    /// Internal implementation of the DatabaseContext
    /// </summary>
    public class DatabaseContext : IDatabaseContext
    {
        readonly IList<IQueryCommand> _queryCommandStore;
        readonly ILoggerFactory _loggerFactory;

        public DatabaseContext(IContextProvider provider, ILoggerFactory loggerFactory)
        {
            ContextProvider = provider;
            _queryCommandStore = new List<IQueryCommand>();
            _loggerFactory = loggerFactory;
        }

        public IContextProvider ContextProvider { get; private set; }

        public void Commit()
        {
            var command = QueryCommandStore.FirstOrDefault();
            while (command != null)
            {
                command.Execute(this);

                _queryCommandStore.Remove(command);
                command = QueryCommandStore.FirstOrDefault();
            }
        }

        public void AddQuery(IQueryCommand command)
        {
            _queryCommandStore.Add(command);
        }

        public IEnumerable<IQueryCommand> QueryCommandStore
        {
            get
            {
                return _queryCommandStore;
            }
        }
        

        private QueryKernel _kernel;
        /// <summary>
        /// Gets or sets a MappingStrategy
        /// </summary>
        public QueryKernel Kernel
        {
            get
            {
                if (_kernel == null)
                    _kernel = new QueryKernel(ContextProvider, _loggerFactory);

                return _kernel;
            }
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
                    // commit all uncommited transactions
                    Commit();

                    ContextProvider.Dispose();

                    IsDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Releases resources before the object is reclaimed by garbage collection.
        /// </summary>
        ~DatabaseContext()
        {
            Dispose(false);
        }

        #endregion
    }
}
