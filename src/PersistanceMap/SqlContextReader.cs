using System.Data;
using System.Data.SqlClient;

namespace PersistanceMap
{
    /// <summary>
    /// Implementation of IReaderContext for SQL Databases
    /// </summary>
    public class SqlContextReader : ReaderContext, IReaderContext
    {
        readonly SqlConnection _connection;
        readonly SqlCommand _command;

        public SqlContextReader(IDataReader reader, SqlConnection connection, SqlCommand command)
            : base(reader)
        {
            _connection = connection;
            _command = command;
        }
                
        public override void Close()
        {
            if (DataReader != null)
                DataReader.Close();

            _connection.Close();
            _command.Dispose();
        }

        #region IDisposeable Implementation

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        public override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing && !IsDisposed)
                {
                    Close();
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
