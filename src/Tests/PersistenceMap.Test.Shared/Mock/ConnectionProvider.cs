using System;

namespace PersistenceMap.Mock
{
    public class ConnectionProvider : PersistenceMap.ConnectionProvider, PersistenceMap.IConnectionProvider
    {
        private readonly Action<string> _onExecute;

        public ConnectionProvider()
            : base(null, null)
        {
            CheckCallbackCall = true;
            QueryCompiler = new QueryCompiler();
        }

        public ConnectionProvider(Action<string> onExecute)
            : base(null, null)
        {
            CheckCallbackCall = true;
            QueryCompiler = new QueryCompiler();

            _onExecute = onExecute;
        }

        public bool CheckCallbackCall { get; set; }

        public override IDataReaderContext Execute(string query)
        {
            ExecuteNonQuery(query);

            return new DataReaderContext();
        }

        public override int ExecuteNonQuery(string query)
        {
            if (_onExecute != null)
            {
                _onExecute(query);
            }

            return 0;
        }
    }
}
