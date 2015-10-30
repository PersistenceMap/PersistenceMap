using PersistanceMap.Tracing;
using System;
using System.Data;

namespace PersistanceMap.Test
{
    /// <summary>
    /// Represents a IContextProvider that only compares the generated sql string to a expected sql string withou executing to a database
    /// </summary>
    public class MockedContextProvider : ContextProvider, IContextProvider
    {
        public MockedContextProvider()
            : base(new MockedConnectionProvider())
        {
        }

        public MockedContextProvider(Action<string> onExecute)
            : base(new MockedConnectionProvider(onExecute))
        {
        }

        public virtual DatabaseContext Open()
        {
            return new DatabaseContext(ConnectionProvider, new LoggerFactory(), Interceptors);
        }

        public class MockedConnectionProvider : ConnectionProvider, IConnectionProvider
        {
            private readonly Action<string> _onExecute;
            private bool _callbackCalled = false;

            public MockedConnectionProvider()
                : base(null, null)
            {
                CheckCallbackCall = true;
                QueryCompiler = new QueryCompiler();
            }

            public MockedConnectionProvider(Action<string> onExecute)
                : base(null, null)
            {
                CheckCallbackCall = true;
                QueryCompiler = new QueryCompiler();

                _onExecute = onExecute;
            }

            public bool CheckCallbackCall { get; set; }
            
            public override IReaderContext Execute(string query)
            {
                ExecuteNonQuery(query);

                return new MockedReaderContext();
            }

            public override void ExecuteNonQuery(string query)
            {
                if (_onExecute != null)
                {
                    _onExecute(query);
                }

                _callbackCalled = true;
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
                        if (CheckCallbackCall && _callbackCalled == false)
                            throw new Exception("Callback was not called by client");

                        IsDisposed = true;
                        GC.SuppressFinalize(this);
                    }
                }
            }

            /// <summary>
            /// Releases resources before the object is reclaimed by garbage collection.
            /// </summary>
            ~MockedConnectionProvider()
            {
                Dispose(false);
            }

            #endregion
        }

        public class MockedReaderContext : IReaderContext
        {
            public System.Data.IDataReader DataReader
            {
                get
                {
                    return new MockedDataReader();
                }
            }

            public void Close()
            {
            }

            public void Dispose()
            {
            }
        }

        public class MockedDataReader : IDataReader
        {
            public void Close()
            {
            }

            public int Depth
            {
                get { return 0; }
            }

            public DataTable GetSchemaTable()
            {
                throw new NotImplementedException();
            }

            public bool IsClosed
            {
                get { return true; }
            }

            public bool NextResult()
            {
                return false;
            }

            public bool Read()
            {
                return false;
            }

            public int RecordsAffected
            {
                get { return 0; }
            }

            public void Dispose()
            {
            }

            public int FieldCount
            {
                get { return 0; }
            }

            #region Not Implemented

            public bool GetBoolean(int i)
            {
                throw new NotImplementedException();
            }

            public byte GetByte(int i)
            {
                throw new NotImplementedException();
            }

            public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
            {
                throw new NotImplementedException();
            }

            public char GetChar(int i)
            {
                throw new NotImplementedException();
            }

            public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
            {
                throw new NotImplementedException();
            }

            public IDataReader GetData(int i)
            {
                throw new NotImplementedException();
            }

            public string GetDataTypeName(int i)
            {
                throw new NotImplementedException();
            }

            public DateTime GetDateTime(int i)
            {
                throw new NotImplementedException();
            }

            public decimal GetDecimal(int i)
            {
                throw new NotImplementedException();
            }

            public double GetDouble(int i)
            {
                throw new NotImplementedException();
            }

            public Type GetFieldType(int i)
            {
                throw new NotImplementedException();
            }

            public float GetFloat(int i)
            {
                throw new NotImplementedException();
            }

            public Guid GetGuid(int i)
            {
                throw new NotImplementedException();
            }

            public short GetInt16(int i)
            {
                throw new NotImplementedException();
            }

            public int GetInt32(int i)
            {
                throw new NotImplementedException();
            }

            public long GetInt64(int i)
            {
                throw new NotImplementedException();
            }

            public string GetName(int i)
            {
                throw new NotImplementedException();
            }

            public int GetOrdinal(string name)
            {
                throw new NotImplementedException();
            }

            public string GetString(int i)
            {
                throw new NotImplementedException();
            }

            public object GetValue(int i)
            {
                throw new NotImplementedException();
            }

            public int GetValues(object[] values)
            {
                throw new NotImplementedException();
            }

            public bool IsDBNull(int i)
            {
                throw new NotImplementedException();
            }

            public object this[string name]
            {
                get { throw new NotImplementedException(); }
            }

            public object this[int i]
            {
                get { throw new NotImplementedException(); }
            }

            #endregion
        }
    }
}
