using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;
using System.Linq;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class FromTests : TestBase
    {
        [Test]
        public void SimpleFromTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var query = context.From<Products>();

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<Products>().Flatten(), "select ProductID, ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued from Products");

                // execute the query
                var prsAbt = query.Select<Products>();               

                Assert.IsTrue(prsAbt.Any());
            }
        }

        [Test]
        public void FromWithIdentifierTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var query = context.From<Products>("prod");

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<Products>().Flatten(), "select ProductID, ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued from Products prod");

                // execute the query
                var products = query.Select<Products>();

                Assert.IsTrue(products.Any());
            }
        }

        [Test]
        public void FromWithJoinAndIdentifiersTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                // join using identifiers in the on expression
                var query = context.From<Orders>("orders")
                    .Map(o => o.OrderID)
                    .Join<OrderDetails>("detail", "orders", (det, order) => det.OrderID == order.OrderID);

                var sql = "select orders.OrderID, ProductID, UnitPrice, Quantity, Discount from Orders orders join OrderDetails detail on (detail.OrderID = orders.OrderID)";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderDetails>().Flatten(), sql);

                // execute the query
                var orders = query.Select<OrderDetails>();

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        public void FromWithIncludeTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                // multiple joins using On<T> with include and includeoperation in from expression
                var query = context
                    .From<Orders>()
                    .Map(p => p.OrderID)
                    .Join<OrderDetails>((det, order) => det.OrderID == order.OrderID)
                    .Join<Products>((product, det) => product.ProductID == det.ProductID)
                    .Map(p => p.ProductID)
                    .Map(p => p.UnitPrice);

                var sql = "select Orders.OrderID, Products.ProductID, Products.UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID) join Products on (Products.ProductID = OrderDetails.ProductID)";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderDetails>().Flatten(), sql);

                // execute the query
                var orders = query.Select<OrderDetails>();

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        public void FromWithIncludeAndIdentifierTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                // multiple joins using On<T> with include and include-operation in from expression with identifier
                var query = context
                    .From<Orders>("ord")
                    .Map(p => p.OrderID)
                    //TODO: Join has to check the previous for the alias! "ord"
                    .Join<OrderDetails>((det, order) => det.OrderID == order.OrderID)
                    .Join<Products>((product, det) => product.ProductID == det.ProductID)
                    .Map(p => p.ProductID)
                    .Map(p => p.UnitPrice);

                //TODO: complete test!
                var sql = "";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderDetails>().Flatten(), sql);

                // execute the query
                var orders = query.Select<OrderDetails>();

                Assert.IsTrue(orders.Any());
            }
        }
    }
}
