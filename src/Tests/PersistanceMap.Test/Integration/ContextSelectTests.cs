using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;
using System.Configuration;
using System.Linq;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class ContextSelectTests : TestBase
    {

        [Test]
        public void SimpleSelectTest()
        {
            //var dbConnection = new DatabaseConnection(new SqlContextProvider("data source=.;initial catalog=Northwind;persist security info=False;user id=sa"));
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var orders = context.Select<Orders>();
                // select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders

                Assert.IsNotNull(orders);
                Assert.IsTrue(orders.Any());
            }
        }
    }
}
