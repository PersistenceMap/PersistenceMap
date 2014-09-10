using NUnit.Framework;
using PersistanceMap.Compiler;

namespace PersistanceMap.Test
{
    public class MockSqlContextProvider : IContextProvider
    {
        public MockSqlContextProvider(string connectionString)
            : this(connectionString, null)
        {
        }

        public MockSqlContextProvider(string connectionString, string expectedResult)
        {
            Assert.IsNotNullOrEmpty(connectionString);
            ConnectionString = connectionString;
            ExpectedResult = expectedResult;
        }

        public string ConnectionString { get; private set; }

        public string ExpectedResult { get; set; }

        private IExpressionCompiler _expressionCompiler;
        public virtual IExpressionCompiler ExpressionCompiler
        {
            get
            {
                if (_expressionCompiler == null)
                    _expressionCompiler = new ExpressionCompiler();

                return _expressionCompiler;
            }
        }

        public IReaderContext Execute(string query)
        {
            Assert.AreEqual(query.Flatten(), ExpectedResult);
            return null;
        }

        public IReaderContext ExecuteNonQuery(string query)
        {
            Assert.AreEqual(query.Flatten(), ExpectedResult);
            return null;
        }
    }
}
