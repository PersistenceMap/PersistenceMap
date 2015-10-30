using System;
using System.Data;

namespace PersistenceMap
{
    /// <summary>
    /// Class providing common implementation for IReaderContext
    /// </summary>
    public class ReaderContext : IReaderContext
    {
        readonly IDbConnection _connection;
        readonly IDbCommand _command;

        public ReaderContext(IDataReader reader)
            : this(reader, null, null)
        {
        }

        public ReaderContext(IDataReader reader, IDbConnection connection, IDbCommand command)
        {
            DataReader = reader;
            _connection = connection;
            _command = command;
        }

        /// <summary>
        /// The datareader that was returned from the database
        /// </summary>
        public IDataReader DataReader { get; private set; }

        /// <summary>
        /// Close all connections to the reader and the database
        /// </summary>
        public virtual void Close()
        {
            if (DataReader != null)
            {
                DataReader.Close();
            }

            if (_connection != null)
            {
                _connection.Close();
            }

            if (_command != null)
            {
                _command.Dispose();
            }
        }

        #region IDisposeable Implementation

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

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
                    Close();

                    IsDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Releases resources before the object is reclaimed by garbage collection.
        /// </summary>
        ~ReaderContext()
        {
            Dispose(false);
        }

        #endregion
    }
}
