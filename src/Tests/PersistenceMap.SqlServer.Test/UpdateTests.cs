using NUnit.Framework;
using PersistenceMap.Test.TableTypes;
using System.Linq;

namespace PersistenceMap.Test.Integration
{
    [TestFixture]
    public class UpdateTests : TestBase
    {
        [Test]
        public void UpdateIntegrationTest()
        {
            var logger = new MessageStackLogWriter();
            var provider = new SqlContextProvider(ConnectionString);
            provider.Settings.AddLogger(logger);
            using (var context = provider.Open())
            {
                context.Update<Orders>(() => new { Freight = 20 }, o => o.OrdersID == 10000000);
                context.Commit();

                Assert.AreEqual(logger.Logs.First().Message.Flatten(), "UPDATE Orders SET Freight = 20 WHERE (Orders.OrdersID = 10000000)");
            }
        }
    }
}
