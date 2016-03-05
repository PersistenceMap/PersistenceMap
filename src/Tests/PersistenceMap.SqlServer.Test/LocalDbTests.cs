using System.IO;
using System.Linq;
using PersistenceMap.Test;
using PersistenceMap.Test.LocalDb;
using PersistenceMap.Test.TableTypes;
using NUnit.Framework;

namespace PersistenceMap.SqlServer.Test
{
    //[SetUpFixture]
    public class LocalDbTests
    {
        private static LocalDbManager _localDbManager;

        [OneTimeSetUp]
        public static void AssemblyInit()
        {
            _localDbManager = new LocalDbManager("Northwind");

            //var file = new FileInfo(@"AppData\Nothwind.SqlServer.sql");
            //string script = file.OpenText().ReadToEnd();
            //_localDbManager.ExecuteString(script);
        }

        [OneTimeTearDown]
        public static void AssemblyCleanup()
        {
            _localDbManager.Dispose();
        }

        [Test]
        public void TestWithLocalDbTest()
        {
            var provider = new SqlContextProvider(_localDbManager.ConnectionString);
            using (var context = provider.Open())
            {
                var file = new FileInfo(@"AppData\Nothwind.SqlServer.sql");
                string script = file.OpenText().ReadToEnd();
                context.Execute(script);

                var query = context.From<Orders>().Map(o => o.OrdersID).Join<OrderDetails>((d, o) => d.OrdersID == o.OrdersID);

                var sql = "SELECT Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";
                var expected = query.CompileQuery();

                // check the compiled sql
                Assert.AreEqual(expected.Flatten(), sql);

                // execute the query
                var orders = query.Select();

                Assert.IsNotNull(orders);
                Assert.IsTrue(orders.Any());
            }
        }
    }
}
