using System;
using System.Data;
using System.Data.SqlClient;

namespace PersistanceMap.Internals
{
    /// <summary>
    /// Implementation of IReaderContext for SQL Databases
    /// </summary>
    internal class SqlContextReader : ReaderContext, IReaderContext
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
            DataReader.Close();
            _connection.Close();
            _command.Dispose();
        }


        public override void SetValue(FieldDefinition fieldDef, int colIndex, object instance)
        {
            //if (fieldDef.IsRowVersion)
            //{
            //    var bytes = DataReader.GetValue(colIndex) as byte[];
            //    if (bytes != null)
            //    {
            //        var ulongValue = ConvertToULong(bytes);
            //        try
            //        {
            //            fieldDef.SetValueFn(instance, ulongValue);
            //        }
            //        catch (NullReferenceException ignore) { }
            //    }
            //}
            //else
            //{
                base.SetValue(fieldDef, colIndex, instance);
            //}
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
