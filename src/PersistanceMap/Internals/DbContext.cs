using PersistanceMap.Mapping;
using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace PersistanceMap.Internals
{
    /// <summary>
    /// Internal implementation of the Database Context
    /// </summary>
    internal class DbContext : IDbContext
    {
        public DbContext(IContextProvider provider)
        {
            ContextProvider = provider;
        }

        public IContextProvider ContextProvider { get; private set; }

        public IEnumerable<T> Execute<T>(CompiledQuery compiledQuery)
        {
            using (var reader = ContextProvider.Execute(compiledQuery.QueryString))
            {
                var mapper = new MappingStrategy();
                return mapper.Map<T>(reader);
            }
        }

        public void Execute(CompiledQuery compiledQuery)
        {
            using (var reader = ContextProvider.Execute(compiledQuery.QueryString))
            {
                // make sure Disposed is called on reader!
            }
        }

        public void Execute(CompiledQuery compiledQuery, params Expression<Action<IDataReader>>[] expressions)
        {
            using (var reader = ContextProvider.Execute(compiledQuery.QueryString))
            {
                foreach (var expression in expressions)
                {
                    expression.Compile().Invoke(reader.DataReader);
                    if(!reader.DataReader.NextResult())
                        break;
                }
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
                    IsDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Releases resources before the object is reclaimed by garbage collection.
        /// </summary>
        ~DbContext()
        {
            Dispose(false);
        }

        #endregion
    }
}
