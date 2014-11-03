using NUnit.Framework;
using PersistanceMap.Test.TableTypes;
using System.Linq;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class UpdateTests : TestBase
    {
        [Test]
        public void UpdateIntegrationTest()
        {
            var logger = new MessageStackLogger();
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            connection.Settings.AddLogger(logger);
            using (var context = connection.Open())
            {
                context.Update<Orders>(() => new { Freight = 20 }, o => o.OrdersID == 10000000);
                context.Commit();

                Assert.AreEqual(logger.Logs.First().Message.Flatten(), "UPDATE Orders SET Freight = 20 where (Orders.OrdersID = 10000000)");
            }
        }
    }
}
