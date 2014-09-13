using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersistanceMap
{
    /// <summary>
    /// Internal implementation of the Database Context
    /// </summary>
    public class DatabaseContext : IDatabaseContext
    {
        public DatabaseContext(IContextProvider provider)
        {
            ContextProvider = provider;
        }

        public IContextProvider ContextProvider { get; private set; }

        public IEnumerable<T> Execute<T>(CompiledQuery compiledQuery)
        {
            using (var reader = ContextProvider.Execute(compiledQuery.QueryString))
            {
                return this.Map<T>(reader);
            }
        }

        public void Execute(CompiledQuery compiledQuery)
        {
            using (var reader = ContextProvider.Execute(compiledQuery.QueryString))
            {
                // make sure Disposed is called on reader!
            }
        }

        public void Execute(CompiledQuery compiledQuery, params Action<IReaderContext>[] expressions)
        {
            using (var reader = ContextProvider.Execute(compiledQuery.QueryString))
            {
                foreach (var expression in expressions)
                {
                    // invoke expression with the reader
                    expression.Invoke(reader);

                    // read next resultset
                    if(reader.DataReader.IsClosed || !reader.DataReader.NextResult())
                        break;
                }
            }
        }

        public void Commit()
        {
            var command = QueryCommandStore.FirstOrDefault();
            while (command != null)
            {
                command.Execute(this);

                QueryCommandStore.Remove(command);
                command = QueryCommandStore.FirstOrDefault();
            }
        }

        public void AddQuery(IQueryCommand command)
        {
            QueryCommandStore.Add(command);
        }

        IList<IQueryCommand> _queryCommandStore;
        public IList<IQueryCommand> QueryCommandStore
        {
            get
            {
                if (_queryCommandStore == null)
                    _queryCommandStore = new List<IQueryCommand>();
                return _queryCommandStore;
            }
        }

        IEnumerable<IQueryCommand> IDatabaseContext.QueryCommandStore
        {
            get
            {
                return QueryCommandStore;
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
                    _kernel = new QueryKernel();

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
