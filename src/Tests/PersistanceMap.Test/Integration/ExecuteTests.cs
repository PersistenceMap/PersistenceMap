using NUnit.Framework;
using PersistanceMap.Test.TableTypes;
using System.Linq;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class ExecuteTests : TestBase
    {
        [Test]
        public void ExecuteSelectStatement()
        {
            var logger = new MessageStackLogger();
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            connection.Settings.AddLogger(logger);
            using (var context = connection.Open())
            {
                // select with string select statement
                var orders = context.Execute<Orders>("SELECT * FROM Orders");

                Assert.IsTrue(orders.Any());
                Assert.AreEqual(logger.Logs.First().Message.Flatten(), "SELECT * FROM Orders");
            }
        }

        [Test]
        public void ExecuteUpateStatement()
        {
            var logger = new MessageStackLogger();
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            connection.Settings.AddLogger(logger);
            using (var context = connection.Open())
            {
                // select with string select statement
                context.Execute("UPDATE Orders SET Freight = 20 WHERE OrdersID = 10000000");

                Assert.AreEqual(logger.Logs.First().Message.Flatten(), "UPDATE Orders SET Freight = 20 WHERE OrdersID = 10000000");
            }
        }
    }
}
