using System;

namespace PersistenceMap.Mock
{
    /// <summary>
    /// Represents a IContextProvider that only compares the generated sql string to a expected sql string withou executing to a database
    /// </summary>
    [Obsolete("Don't use MockContextProvider any more")]
    public class MockContextProvider : ContextProvider, IContextProvider
    {
        [Obsolete("Don't use MockContextProvider any more")]
        public MockContextProvider()
            : base(new PersistenceMap.Mock.ConnectionProvider())
        {
        }

        [Obsolete("Don't use MockContextProvider any more")]
        public MockContextProvider(Action<string> onExecute)
            : base(new PersistenceMap.Mock.ConnectionProvider(onExecute))
        {
        }

        public virtual DatabaseContext Open()
        {
            return new DatabaseContext(ConnectionProvider, new Settings(), Interceptors);
        }
    }
}
