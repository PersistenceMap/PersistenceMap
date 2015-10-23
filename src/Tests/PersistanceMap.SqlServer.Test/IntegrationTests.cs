using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistanceMap.Test;
using PersistanceMap.Test.LocalDb;
using PersistanceMap.Test.TableTypes;

namespace PersistanceMap.SqlServer.Test
{
    //[TestClass]
    public class IntegrationTests : TestBase
    {
        private static LocalDbManager _localDbManager;

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            _localDbManager = new LocalDbManager("Northwind");

            //var file = new FileInfo(@"AppData\Nothwind.SqlServer.sql");
            //string script = file.OpenText().ReadToEnd();
            //_localDbManager.ExecuteString(script);
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            _localDbManager.Dispose();
        }

        [TestMethod]
        public void LocalDbTest()
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
