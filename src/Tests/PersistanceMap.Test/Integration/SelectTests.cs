using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;
using System.Linq;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class SelectTests : TestBase
    {

        [Test]
        public void SimpleSelectTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var orders = context.Select<Orders>();
                /* *Expected Query*
                select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry 
                from Orders
                */

                Assert.IsNotNull(orders);
                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        public void SimpleNongenericSelectTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var query = context.From<Orders>();

                var sql = "select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<Orders>().Flatten(), sql);

                // execute the query
                var orders = query.Select();

                Assert.IsNotNull(orders);
                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        public void SimpleNongenericSelectWithJoinTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var query = context.From<Orders>()
                    .Map(o => o.OrderID)
                    .Join<OrderDetails>((d, o) => d.OrderID == o.OrderID);

                var sql = "select Orders.OrderID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderDetails>().Flatten(), sql);

                // execute the query
                var orders = query.Select();
                
                Assert.IsNotNull(orders);
                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        public void SelectWithMapping()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // Map => To 
                var query = context.From<Orders>()
                    .Map<OrderWithDetailExtended, double>(source => source.Freight, alias => alias.SpecialFreight)
                    .Join<OrderDetails>((detail, order) => detail.OrderID == order.OrderID)
                    // map a property from a joni to a property in the result type
                    .Map(i => i.OrderID);

                var sql = "select Orders.Freight as SpecialFreight, OrderDetails.OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderWithDetailExtended>().Flatten(), sql);

                // execute the query
                var orders = query.Select<OrderWithDetailExtended>();

                Assert.IsTrue(orders.Any());
                Assert.IsTrue(orders.First().SpecialFreight > 0);
            }
        }

        [Test]
        public void SelectWithExtendedMapping()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // Map => To 
                var query = context.From<Orders>()
                    .Join<OrderDetails>((detail, order) => detail.OrderID == order.OrderID)
                    .Map(i => i.OrderID)
                    // map a property from a joni to a property in the result type
                    .Map<Orders, OrderWithDetailExtended, double>(source => source.Freight, alias => alias.SpecialFreight);

                var sql = "select OrderDetails.OrderID, Orders.Freight as SpecialFreight, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderWithDetailExtended>().Flatten(), sql);

                // execute the query
                var orders = query.Select<OrderWithDetailExtended>();
                
                Assert.IsTrue(orders.Any());
                Assert.IsTrue(orders.First().SpecialFreight > 0);
            }
        }

        [Test]
        public void Select_WithStringSelectStatement()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // select with string select statement
                var orders = context.Select<Orders>("Select * from Orders");

                Assert.IsTrue(orders.Any());
            }
        }
    }
}
