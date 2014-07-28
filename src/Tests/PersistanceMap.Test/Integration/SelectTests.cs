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
                var orders = context.From<Orders>().Select();

                /* *Expected Query*
                select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry 
                from Orders 
                */

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
                var orders = context.From<Orders>()
                    .Include(o => o.OrderID)
                    .Join<OrderDetails>((d, o) => d.OrderID == o.OrderID)
                    .Select();

                /* *Expected Query*
                select Orders.OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry 
                from Orders 
                join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)
                */

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
                var owd = context.From<Orders>()
                    .Map<OrderWithDetailExtended, double>(source => source.Freight, alias => alias.SpecialFreight)
                    .Join<OrderDetails>((detail, order) => detail.OrderID == order.OrderID)
                    .Include(i => i.OrderID)                    
                    // map a property from a joni to a property in the result type
                    .Select<OrderWithDetailExtended>();

                /* *Expected Query*
                select ..., Orders.Freight as SpecialFreight, ... 
                from Orders 
                join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)
                */

                Assert.IsTrue(owd.Any());
                Assert.IsTrue(owd.First().SpecialFreight > 0);
            }
        }

        [Test]
        public void SelectWithExtendedMapping()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // Map => To 
                var owd = context.From<Orders>()
                    .Join<OrderDetails>((detail, order) => detail.OrderID == order.OrderID)
                    .Include(i => i.OrderID)
                    // map a property from a joni to a property in the result type
                    .Map<Orders, OrderWithDetailExtended, double>(source => source.Freight, alias => alias.SpecialFreight)
                    .Select<OrderWithDetailExtended>();

                /* *Expected Query*
                select ..., Orders.Freight as SpecialFreight, ... 
                from Orders 
                join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)
                */

                Assert.IsTrue(owd.Any());
                Assert.IsTrue(owd.First().SpecialFreight > 0);
            }
        }
    }
}
