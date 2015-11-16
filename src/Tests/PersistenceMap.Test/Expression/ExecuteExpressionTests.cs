using NUnit.Framework;
using PersistenceMap.Mock;
using PersistenceMap.Test.TableTypes;

namespace PersistenceMap.Test.Expression
{
    [TestFixture]
    public class ExecuteExpressionTests
    {
        [Test]
        public void ExecuteSelectStatement()
        {
            var provider = new ContextProvider(new Mock.ConnectionProvider());
            provider.Interceptor<Orders>().BeforeExecute(s => Assert.AreEqual(s.QueryString.Flatten(), "SELECT * FROM Orders"));
            using (var context = provider.Open())
            {
                // select with string select statement
                var orders = context.Execute<Orders>("SELECT * FROM Orders");
            }
        }

        [Test]
        public void ExecuteSqlStatement()
        {
            var provider = new ContextProvider(new Mock.ConnectionProvider());
            provider.Interceptor<Orders>().BeforeExecute(s => Assert.AreEqual(s.QueryString.Flatten(), "UPDATE Orders SET Freight = 20 WHERE OrdersID = 10000000"));
            using (var context = provider.Open())
            {
                // select with string select statement
                context.Execute("UPDATE Orders SET Freight = 20 WHERE OrdersID = 10000000");
            }
        }
    }
}
