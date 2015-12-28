using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistenceMap.Samples.UnitOfWorkSample
{
    class UnitOfWork : IDisposable
    {
        readonly SqliteDatabaseContext _context;

        public UnitOfWork(SqliteDatabaseContext context)
        {
            _context = context;
        }

        public UnitOfWork(string connectionString)
        {
            var connection = new SqliteContextProvider(connectionString);
            _context = connection.Open();
        }

        public SqliteDatabaseContext Context
        {
            get
            {
                return _context;
            }
        }

        public void Commit()
        {
            Context.Commit();
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
                    _context.Commit();
                    _context.Dispose();

                    IsDisposed = true;
                }
            }
        }
        
        #endregion
    }
}
