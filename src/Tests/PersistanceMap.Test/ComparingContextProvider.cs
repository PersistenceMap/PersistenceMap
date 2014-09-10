using NUnit.Framework;
using PersistanceMap.Compiler;

namespace PersistanceMap.Test
{
    /// <summary>
    /// Represents a IContextProvider that only compares the generated sql string to a expected sql string withou executing to a database
    /// </summary>
    public class ComparingContextProvider : IContextProvider
    {
        public ComparingContextProvider(string connectionString)
            : this(connectionString, null)
        {
        }

        public ComparingContextProvider(string connectionString, string expectedResult)
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
