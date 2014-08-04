using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;
using System.Linq;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class FromTests : TestBase
    {
        [Test]
        public void Select_From_Direct()
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
        public void Select_From_WithAliasInFrom()
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
        public void Select_FromW_ithJoin_WithAlias()
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
        public void Select_From_WithAlias()
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

                var sql = query.CompileQuery<OrderDetails>().Flatten();
                var expected = "select Orders.OrderID, Products.ProductID, Products.UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID) join Products on (Products.ProductID = OrderDetails.ProductID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                // execute the query
                var orders = query.Select<OrderDetails>();

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("A select statement where the map takes the alias from the previous operation")]
        public void From_WithInclude_WithAlias_MapWithoutAlias()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var query = context
                    .From<Orders>("ord")
                    .Map(p => p.OrderID)
                    //TODO: Join has to check the previous for the alias? "ord"
                    .Join<OrderDetails>(null, "ord", (det, order) => det.OrderID == order.OrderID)
                    .Join<Products>((product, det) => product.ProductID == det.ProductID)
                    .Map(p => p.ProductID)
                    .Map(p => p.UnitPrice);

                var sql = query.CompileQuery<OrderDetails>().Flatten();
                var expected = "select ord.OrderID, Products.ProductID, Products.UnitPrice, Quantity, Discount from Orders ord join OrderDetails on (OrderDetails.OrderID = ord.OrderID) join Products on (Products.ProductID = OrderDetails.ProductID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                // execute the query
                var orders = query.Select<OrderDetails>();

                Assert.IsTrue(orders.Any());
            }
        }

        //[Test]
        //[Description("A select statement with the aliases that are defined in only the first map")]
        //public void FromWithIncludeAndIdentifierTest()
        //{
        //    var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
        //    using (var context = dbConnection.Open())
        //    {
        //        // multiple joins using On<T> with include and include-operation in from expression with identifier
        //        var query = context
        //            .From<Orders>("ord")
        //            .Map(p => p.OrderID)
        //            //TODO: Join has to check the previous for the alias! "ord"
        //            .Join<OrderDetails>((det, order) => det.OrderID == order.OrderID)
        //            .Join<Products>((product, det) => product.ProductID == det.ProductID)
        //            .Map(p => p.ProductID)
        //            .Map(p => p.UnitPrice);

        //        //TODO: complete test!
        //        var sql = "";

        //        // check the compiled sql
        //        Assert.AreEqual(query.CompileQuery<OrderDetails>().Flatten(), sql);

        //        // execute the query
        //        var orders = query.Select<OrderDetails>();

        //        Assert.IsTrue(orders.Any());
        //    }
        //}
    }
}
