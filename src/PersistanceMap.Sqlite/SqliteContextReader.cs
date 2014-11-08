using System.Data;

namespace PersistanceMap.Sqlite
{
    /// <summary>
    /// Implementation of IReaderContext for SQL Server Compact Databases
    /// </summary>
    public class SqliteContextReader : ReaderContext, IReaderContext
    {
        readonly IDbConnection _connection;
        readonly IDbCommand _command;

        public SqliteContextReader(IDataReader reader, IDbConnection connection, IDbCommand command)
            : base(reader)
        {
            _connection = connection;
            _command = command;
        }
                
        public override void Close()
        {
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
