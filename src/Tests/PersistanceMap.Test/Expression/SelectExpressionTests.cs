using NUnit.Framework;
using PersistanceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.Test.Expression
{
    [TestFixture]
    public class SelectExpressionTests
    {
        [Test]
        public void SelectWithAliasMapping()
        {
            var expected = "select Orders.Freight as SpecialFreight, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

            var connection = new DatabaseConnection(new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected)));
            using (var context = connection.Open())
            {
                // Map => To 
                context.From<Orders>()
                    .Map<OrderWithDetailExtended>(source => source.Freight, alias => alias.SpecialFreight)
                    .Join<OrderDetails>((detail, order) => detail.OrdersID == order.OrdersID)
                    // map a property from a join to a property in the result type
                    .Map(i => i.OrdersID)
                    .Select<OrderWithDetailExtended>();
            }
        }

        [Test]
        public void SelectWithExtendedMapping()
        {
            var expected = "select Orders.Freight as SpecialFreight, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";
            
            var connection = new DatabaseConnection(new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected)));
            using (var context = connection.Open())
            {
                // Map => To 
                context.From<Orders>()
                    .Join<OrderDetails>((detail, order) => detail.OrdersID == order.OrdersID)
                    .Map(i => i.OrdersID)
                    // map a property from a joni to a property in the result type
                    .Map<Orders, OrderWithDetailExtended>(source => source.Freight, alias => alias.SpecialFreight)
                    .Select<OrderWithDetailExtended>();
            }
        }

        [Test(Description = "Select with a anonym object definition")]
        public void SelectAnonymObjectTypeDefiniton()
        {
            var expected = "select ProductID, Quantity from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

            var connection = new DatabaseConnection(new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected)));
            using (var context = connection.Open())
            {
                context.From<Orders>()
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)
                    .Select(() => new
                    {
                        ProductID = 0,
                        Quantity = 0.0
                    });
            }
        }

        [Test(Description = "Select to a anonym object")]
        public void SelectAnonymObject()
        {
            var expected = "select Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

            var connection = new DatabaseConnection(new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected)));
            using (var context = connection.Open())
            {
                var items = context.From<Orders>()
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)
                    .Select(od => new
                    {
                        ProductID = od.ProductID,
                        Quantity = od.Quantity
                    });

                items.Any();
            }
        }

        [Test(Description = "Select to a anonym object delegate")]
        public void SelectAnonymObject2()
        {
            var expected = "select Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

            var connection = new DatabaseConnection(new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected)));
            using (var context = connection.Open())
            {
                var items = context.From<Orders>()
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)
                    // select into a anonymous object
                    .Select(od => new
                    {
                        Prud = od.Quantity
                    });

                items.Any();
            }
        }

        [Test(Description = "Select to a type object delegate")]
        public void SelectCustomObjectWithDelegate()
        {
            var expected = "select Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

            var connection = new DatabaseConnection(new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected)));
            using (var context = connection.Open())
            {
                // select only the properties that are defined in the anony object
                var items = context.From<Orders>()
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)
                    .Select(od => new OrderWithDetail
                    {
                        // only select the properties defined
                        ProductID = od.ProductID,
                        Quantity = od.Quantity
                    });

                items.Any();
            }
        }

        [Test]
        [Description("select statement that compiles from a FOR operation with a anonym object defining the resultset entries")]
        public void Select_For_Anonym_ObjectType()
        {
            var expected = "select ProductID, Quantity from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

            var connection = new DatabaseConnection(new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected)));
            using (var context = connection.Open())
            {
                // select only the properties that are defined in the anony object
                context.From<Orders>()
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)
                    .For(() => new
                    {
                        ProductID = 0,
                        Quantity = 0
                    })
                    .Select();
            }
        }
    }
}
