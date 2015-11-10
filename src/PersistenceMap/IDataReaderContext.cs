using System;
using System.Data;

namespace PersistenceMap
{
    /// <summary>
    /// Provides a context containing the datareader
    /// </summary>
    public interface IDataReaderContext : IDisposable
    {
        /// <summary>
        /// The datareader that was returned from the database
        /// </summary>
        IDataReader DataReader { get; }

        /// <summary>
        /// Close all connections to the reader and the database
        /// </summary>
        void Close();
    }
}
