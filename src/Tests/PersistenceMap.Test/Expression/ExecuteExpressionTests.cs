using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;
using PersistenceMap.Test.TableTypes;

namespace PersistenceMap.Test.Expression
{
    [TestFixture]
    public class ExecuteExpressionTests
    {
        private Mock<IConnectionProvider> _connectionProvider;

        [SetUp]
        public void SetUp()
        {
            _connectionProvider = new Mock<IConnectionProvider>();
            _connectionProvider.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());
        }

        [Test]
        public void PersistenceMap_Integration_Execute_SelectStatement()
        {
            _connectionProvider.Setup(exp => exp.Execute(It.IsAny<string>())).Returns(new DataReaderContext(null));

            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // select with string select statement
                context.Execute<Orders>("SELECT * FROM Orders");

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT * FROM Orders")), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Execute_SqlStatement()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // select with string select statement
                context.Execute("UPDATE Orders SET Freight = 20 WHERE OrdersID = 10000000");

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "UPDATE Orders SET Freight = 20 WHERE OrdersID = 10000000")), Times.Once);
            }
        }
    }
}
