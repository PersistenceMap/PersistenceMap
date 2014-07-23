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
                var prsAbt = context.From<Products>().Select<Products>();

                /* *Expected Query*
                select ProductID, ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued 
                from Products 
                */

                Assert.IsTrue(prsAbt.Any());
            }
        }

        [Test]
        public void FromWithIdentifierTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var products = context.From<Products>(opt => opt.As(() => "prod")).Select<Products>();

                /* *Expected Query*
                 select prod.ProductID, prod.ProductName, prod.SupplierID, prod.CategoryID, prod.QuantityPerUnit, prod.UnitPrice, prod.UnitsInStock, prod.UnitsOnOrder, prod.ReorderLevel, prod.Discontinued 
                 from Products prod 
                */

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
                var orders = context.From<Orders>(
                        opt => opt.As(() => "orders"), 
                        opt => opt.Include(o => o.OrderID))
                    .Join<OrderDetails>(
                        opt => opt.As(() => "detail"), 
                        opt => opt.On("orders", (det, order) => det.OrderID == order.OrderID))
                    .Select<OrderDetails>();

                /* *Expected Query*
                select orders.OrderID, ProductID, UnitPrice, Quantity, Discount 
                from Orders orders 
                join OrderDetails detail on (detail.OrderID = orders.OrderID)
                */

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
                var orders = context
                    .From<Orders>(opt => opt.Include(p => p.OrderID))
                    .Join<OrderDetails>(opt => opt.On((det, order) => det.OrderID == order.OrderID))
                    .Join<Products>(
                        opt => opt.On<OrderDetails>((product, det) => product.ProductID == det.ProductID),
                        opt => opt.Include(p => p.ProductID),
                        opt => opt.Include(p => p.UnitPrice))
                    .Select<OrderDetails>();

                /* *Expected Query*
                 select Products.ProductID, Products.UnitPrice, Orders.OrderID, Quantity, Discount 
                 from Orders 
                 join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)
                 join Products on (Products.ProductID = OrderDetails.ProductID)
                */

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
                var orders = context
                    .From<Orders>(opt => opt.Include(p => p.OrderID), opt => opt.As(() => "ord"))
                    .Join<OrderDetails>(opt => opt.On("ord", (det, order) => det.OrderID == order.OrderID))
                    .Join<Products>(
                        opt => opt.On<OrderDetails>((product, det) => product.ProductID == det.ProductID),
                        opt => opt.Include(p => p.ProductID),
                        opt => opt.Include(p => p.UnitPrice))
                    .Select<OrderDetails>();

                /* *Expected Query*
                select Products.ProductID, Products.UnitPrice, ord.OrderID, Quantity, Discount 
                from Orders ord 
                join OrderDetails on (OrderDetails.OrderID = ord.OrderID)
                join Products on (Products.ProductID = OrderDetails.ProductID)
                */

                Assert.IsTrue(orders.Any());
            }
        }
    }
}
