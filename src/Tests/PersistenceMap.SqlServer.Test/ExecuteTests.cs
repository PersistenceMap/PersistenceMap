using NUnit.Framework;
using PersistenceMap.Test.TableTypes;
using System.Linq;

namespace PersistenceMap.Test.Integration
{
    [TestFixture]
    public class ExecuteTests : TestBase
    {
        [Test]
        public void ExecuteSelectStatement()
        {
            var logger = new MessageStackLogWriter();
            var provider = new SqlContextProvider(ConnectionString);
            provider.Settings.AddLogger(logger);
            using (var context = provider.Open())
            {
                // select with string select statement
                var orders = context.Execute<Orders>("SELECT * FROM Orders");

                Assert.IsTrue(orders.Any());
                Assert.AreEqual(logger.Logs.First().Message.Flatten(), "SELECT * FROM Orders");
            }
        }

        [Test]
        public void ExecuteAnonymSelectStatement()
        {
            var logger = new MessageStackLogWriter();
            var provider = new SqlContextProvider(ConnectionString);
            provider.Settings.AddLogger(logger);
            using (var context = provider.Open())
            {
                // select with string select statement
                var orders = context.Execute("SELECT * FROM Orders", () => new { OrdersID = 0 });

                Assert.IsTrue(orders.Any());
                Assert.AreEqual(logger.Logs.First().Message.Flatten(), "SELECT * FROM Orders");
            }
        }

        [Test]
        public void ExecuteUpateStatement()
        {
            var logger = new MessageStackLogWriter();
            var provider = new SqlContextProvider(ConnectionString);
            provider.Settings.AddLogger(logger);
            using (var context = provider.Open())
            {
                // select with string select statement
                context.Execute("UPDATE Orders SET Freight = 20 WHERE OrdersID = 10000000");

                Assert.AreEqual(logger.Logs.First().Message.Flatten(), "UPDATE Orders SET Freight = 20 WHERE OrdersID = 10000000");
            }
        }
    }
}
