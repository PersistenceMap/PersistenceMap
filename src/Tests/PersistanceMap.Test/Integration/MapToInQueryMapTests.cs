using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersistanceMap.Test.BusinessObjects;
using System.Collections;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class MapToInQueryMapTests : TestBase
    {
        [Test]
        public void SelectWithMapToInJoinWithTypeSourceAndStringAlias()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // Map => To in join with string
                var query = context.From<Orders>()
                    // map the property from this join to the Property in the result type
                    .Map(source => source.Freight, "SpecialFreight")
                    .Join<OrderDetails>((detail, order) => detail.OrderID == order.OrderID)
                    .Map(i => i.OrderID);

                var sql = "select Orders.Freight as SpecialFreight, OrderDetails.OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderWithDetailExtended>().Flatten(), sql);

                // execute the query
                var orders = query.Select<OrderWithDetailExtended>();

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        public void SelectWithMapToInJoinWithTypeSourceAndTypeAlias()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // Map => To in join with predicate
                var query = context.From<Orders>()
                    // map the property from this join to the Property in the result type
                    .Map<OrderWithDetailExtended, double>(source => source.Freight, alias => alias.SpecialFreight)
                    .Join<OrderDetails>((detail, order) => detail.OrderID == order.OrderID)
                    .Map(i => i.OrderID);

                var sql = "select Orders.Freight as SpecialFreight, OrderDetails.OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderWithDetailExtended>().Flatten(), sql);

                // execute the query
                var orders = query.Select<OrderWithDetailExtended>();

                Assert.IsTrue(orders.Any());
            }
        }

        [Test, TestCaseSource(typeof(MapToTestCases), "MapTestCases")]
        public string WhereTest(IOrderQueryProvider<Orders> query)
        {
            // execute the query
            var orders = query.Select();

            Assert.IsTrue(orders.Any());

            // return the query string
            return query.CompileQuery<Orders>().Flatten();
        }
    }

    class MapToTestCases : TestBase
    {
        public IEnumerable MapTestCases
        {
            get
            {
                var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
                using (var context = connection.Open())
                {
                    yield return new TestCaseData()
                        .Returns("")
                        .SetDescription("")
                        .SetName("");
                }
            }
        }
    }
}
