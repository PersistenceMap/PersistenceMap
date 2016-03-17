using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;
using PersistenceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using PersistenceMap.Test;

namespace PersistenceMap.UnitTest.Expression
{
    [TestFixture]
    public class SelectExpressionTests
    {
        private Mock<IConnectionProvider> _connectionProvider;

        [SetUp]
        public void SetUp()
        {
            _connectionProvider = new Mock<IConnectionProvider>();
            _connectionProvider.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());
            _connectionProvider.Setup(exp => exp.Execute(It.IsAny<string>())).Returns(new DataReaderContext(null));
        }

        [Test]
        public void PersistenceMap_Integration_Select_Expression_SelectWithPredicate()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // ignore a member in the select
                context.From<Warrior>(w => w.ID == 1).Select();

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT ID, Name, WeaponID, Race, SpecialSkill FROM Warrior WHERE (Warrior.ID = 1)")), Times.Once);
                _connectionProvider.ResetCalls();

                // ignore a member in the SELECT
                context.Select<Warrior>(w => w.ID == 1).Any();

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT ID, Name, WeaponID, Race, SpecialSkill FROM Warrior WHERE (Warrior.ID = 1)")), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Select_WithPredicate()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // ignore a member in the select
                context.From<Warrior>(w => w.ID == 1)
                    .Select();

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT ID, Name, WeaponID, Race, SpecialSkill FROM Warrior WHERE (Warrior.ID = 1)")), Times.Once);
                _connectionProvider.ResetCalls();

                // ignore a member in the SELECT
                context.Select<Warrior>(w => w.ID == 1);

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT ID, Name, WeaponID, Race, SpecialSkill FROM Warrior WHERE (Warrior.ID = 1)")), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Select_WithGroupBy()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.From<Warrior>().Ignore(w => w.ID).Ignore(w => w.SpecialSkill).Ignore(w => w.WeaponID).Ignore(w => w.Name).GroupBy(w => w.Race).Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT Race FROM Warrior GROUP BY Race")), Times.Once);
                _connectionProvider.ResetCalls();

                context.From<Warrior>().GroupBy(w => w.Race).ThenBy(w => w.WeaponID).For<Warrior>().Ignore(w => w.ID).Ignore(w => w.SpecialSkill).Ignore(w => w.Name).Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT WeaponID, Race FROM Warrior GROUP BY Race, WeaponID")), Times.Once);
                _connectionProvider.ResetCalls();

                context.From<Warrior>().For(() => new { ID = 0, Race = "" }).GroupBy(w => w.Race).ThenBy(w => w.ID).Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT ID, Race FROM Warrior GROUP BY Race, ID")), Times.Once);
                _connectionProvider.ResetCalls();

                context.From<Warrior>().Join<Weapon>((wep, war) => wep.ID == war.WeaponID).Where(w => w.Damage > 20).GroupBy<Warrior>(w => w.Race).For(() => new { Race = "" }).Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT Race FROM Warrior JOIN Weapon ON (Weapon.ID = Warrior.WeaponID) WHERE (Weapon.Damage > 20) GROUP BY Race")), Times.Once);
                _connectionProvider.ResetCalls();

                context.From<Warrior>().Join<Weapon>((wep, war) => wep.ID == war.WeaponID).Where(w => w.Damage > 20).GroupBy<Warrior>(w => w.Race).ThenBy<Warrior>(w => w.WeaponID).For(() => new { Race = "", WeaponID = 0 }).Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT Race, WeaponID FROM Warrior JOIN Weapon ON (Weapon.ID = Warrior.WeaponID) WHERE (Weapon.Damage > 20) GROUP BY Race, WeaponID")), Times.Once);
                _connectionProvider.ResetCalls();

                context.From<Warrior>().Join<Weapon>((wep, war) => wep.ID == war.WeaponID).Where(w => w.Damage > 20).For(() => new { Race = "" }).GroupBy(w => w.Race).Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT Race FROM Warrior JOIN Weapon ON (Weapon.ID = Warrior.WeaponID) WHERE (Weapon.Damage > 20) GROUP BY Race")), Times.Once);
                _connectionProvider.ResetCalls();

                context.From<Warrior>().Join<Weapon>((wep, war) => wep.ID == war.WeaponID).Where(w => w.Damage > 20).For(() => new { Race = "", WeaponID = 0 }).GroupBy(w => w.Race).ThenBy(w => w.WeaponID).Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT Race, WeaponID FROM Warrior JOIN Weapon ON (Weapon.ID = Warrior.WeaponID) WHERE (Weapon.Damage > 20) GROUP BY Race, WeaponID")), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Select_WithMax()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // select the max id
                context.From<Warrior>().Max(w => w.ID).Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT MAX(ID) AS ID FROM Warrior")), Times.Once);
                _connectionProvider.ResetCalls();

                // SELECT the max id with grouping
                context.From<Warrior>().Max(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT MAX(ID) AS ID, Warrior.Race FROM Warrior GROUP BY Race")), Times.Once);
                _connectionProvider.ResetCalls();

                // select the max id with grouping
                context.From<Warrior>().Max(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).For<Warrior>().Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT MAX(ID) AS ID, Warrior.Race FROM Warrior GROUP BY Race")), Times.Once);
                _connectionProvider.ResetCalls();

                context.From<Warrior>().Max(w => w.ID, "MaxID").For(() => new { MaxID = 0 }).Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT MAX(ID) AS MaxID FROM Warrior")), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Select_WithMin()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // select the min id
                context.From<Warrior>().Min(w => w.ID).Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT MIN(ID) AS ID FROM Warrior")), Times.Once);
                _connectionProvider.ResetCalls();

                // select the min id with grouping
                context.From<Warrior>().Min(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT MIN(ID) AS ID, Warrior.Race FROM Warrior GROUP BY Race")), Times.Once);
                _connectionProvider.ResetCalls();

                // select the min id with grouping
                context.From<Warrior>().Min(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).For<Warrior>().Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT MIN(ID) AS ID, Warrior.Race FROM Warrior GROUP BY Race")), Times.Once);
                _connectionProvider.ResetCalls();

                // select the min id
                context.From<Warrior>().Min(w => w.ID, "MinID").For(() => new { MinID = 0 }).Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT MIN(ID) AS MinID FROM Warrior")), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Select_WithCount()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // select the min id
                context.From<Warrior>().Count(w => w.ID).Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT COUNT(ID) AS ID FROM Warrior")), Times.Once);

                // select the min id with grouping
                context.From<Warrior>().Count(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT COUNT(ID) AS ID, Warrior.Race FROM Warrior GROUP BY Race")), Times.Once);
                _connectionProvider.ResetCalls();

                // select the min id with grouping
                context.From<Warrior>().Count(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).For<Warrior>().Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT COUNT(ID) AS ID, Warrior.Race FROM Warrior GROUP BY Race")), Times.Once);

                // select the min id
                context.From<Warrior>().Count(w => w.ID, "IdCount").For(() => new { IdCount = 0 }).Select();
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT COUNT(ID) AS IdCount FROM Warrior")), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Select_WithAliasMapping()
        {
            var expected = "SELECT Orders.Freight AS SpecialFreight, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // Map => To 
                context.From<Orders>()
                    .Map<OrderWithDetailExtended>(source => source.Freight, alias => alias.SpecialFreight)
                    .Join<OrderDetails>((detail, order) => detail.OrdersID == order.OrdersID)

                    // map a property from a JOIN to a property in the result type
                    .Map(i => i.OrdersID)
                    .Select<OrderWithDetailExtended>();

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Select_WithExtendedMapping()
        {
            var expected = "SELECT Orders.Freight AS SpecialFreight, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // Map => To 
                context.From<Orders>()
                    .Join<OrderDetails>((detail, order) => detail.OrdersID == order.OrdersID)
                    .Map(i => i.OrdersID)

                    // map a property from a joni to a property in the result type
                    .Map<Orders, OrderWithDetailExtended>(source => source.Freight, alias => alias.SpecialFreight)
                    .Select<OrderWithDetailExtended>();

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
            }
        }

        [Test(Description = "Select with a anonym object definition")]
        public void PersistenceMap_Integration_Select_AnonymObjectTypeDefiniton()
        {
            var expected = "SELECT ProductID, Quantity FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";
            
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.From<Orders>()
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)
                    .Select(() => new
                    {
                        ProductID = 0,
                        Quantity = 0.0
                    });

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
            }
        }

        [Test(Description = "Select to a anonym object")]
        public void PersistenceMap_Integration_Select_AnonymObject()
        {
            var expected = "SELECT Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
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

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
            }
        }

        [Test(Description = "Select to a anonym object delegate")]
        public void PersistenceMap_Integration_Select_AnonymObject2()
        {
            var expected = "SELECT Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                var items = context.From<Orders>()
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)

                    // SELECT into a anonymous object
                    .Select(od => new
                    {
                        Prud = od.Quantity
                    });

                items.Any();

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
            }
        }

        [Test(Description = "Select to a type object delegate")]
        public void PersistenceMap_Integration_Select_CustomObjectWithDelegate()
        {
            var expected = "SELECT Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // SELECT only the properties that are defined in the anony object
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
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
            }
        }

        [Test]
        [Description("select statement that compiles from a FOR operation with a anonym object defining the resultset entries")]
        public void PersistenceMap_Integration_Select_ForAnonymObjectType()
        {
            var expected = "SELECT ProductID, Quantity FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
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

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Select_WithINExpression()
        {
            var expected = "SELECT ID, Name, WeaponID, Race, SpecialSkill FROM Warrior WHERE Warrior.Race In ('Elf','Dwarf')";

            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                IEnumerable<string> races = new List<string>
                {
                    "Elf",
                    "Dwarf"
                };

                context.From<Warrior>()
                    .Where(w => races.Contains(w.Race))
                    .Select();

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Select_WithOrderTest()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // join with simple order by
                context.From<Orders>().OrderBy(o => o.OrderDate).Select();
                string expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ORDER BY Orders.OrderDate ASC";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
                _connectionProvider.ResetCalls();

                // join with generic order by
                context.From<Orders>().Join<Customers>((c, o) => c.CustomerID == o.CustomerID).Map(c => c.CustomerID).Map(c => c.EmployeeID).OrderBy<Orders>(o => o.OrderDate).Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN Customers ON (Customers.CustomerID = Orders.CustomerID) ORDER BY Orders.OrderDate ASC";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
                _connectionProvider.ResetCalls();

                // join with simple order by desc
                context.From<Orders>().OrderByDesc(o => o.OrderDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ORDER BY Orders.OrderDate DESC";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
                _connectionProvider.ResetCalls();

                // join with generic order by desc
                context.From<Orders>().Join<Customers>((c, o) => c.CustomerID == o.CustomerID).Map(c => c.CustomerID).Map(c => c.EmployeeID).OrderByDesc<Orders>(o => o.OrderDate).Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN Customers ON (Customers.CustomerID = Orders.CustomerID) ORDER BY Orders.OrderDate DESC";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
                _connectionProvider.ResetCalls();

                // join with simple order by with simple then by
                context.From<Orders>().OrderBy(o => o.OrderDate).ThenBy(o => o.RequiredDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ORDER BY Orders.OrderDate ASC, Orders.RequiredDate ASC";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
                _connectionProvider.ResetCalls();

                // join with generic order by with simple then by
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderBy<Orders>(o => o.OrderDate)
                    .ThenBy(o => o.RequiredDate)
                    .Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN Customers ON (Customers.CustomerID = Orders.CustomerID) ORDER BY Orders.OrderDate ASC, Orders.RequiredDate ASC";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
                _connectionProvider.ResetCalls();

                // join with simple order by desc with simple then by
                context.From<Orders>().OrderByDesc(o => o.OrderDate).ThenBy(o => o.RequiredDate).Select();

                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ORDER BY Orders.OrderDate DESC, Orders.RequiredDate ASC";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
                _connectionProvider.ResetCalls();

                // join with generic order by desc with simple then by
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderByDesc<Orders>(o => o.OrderDate)
                    .ThenBy(o => o.RequiredDate)
                    .Select();

                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN Customers ON (Customers.CustomerID = Orders.CustomerID) ORDER BY Orders.OrderDate DESC, Orders.RequiredDate ASC";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
                _connectionProvider.ResetCalls();

                // join with simple order by with generic then by
                context.From<Orders>().OrderBy(o => o.OrderDate).ThenBy<Orders>(o => o.RequiredDate).Select();

                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ORDER BY Orders.OrderDate ASC, Orders.RequiredDate ASC";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
                _connectionProvider.ResetCalls();

                // join with generic order by with generic then by
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderBy<Orders>(o => o.OrderDate)
                    .ThenBy<Customers>(c => c.CompanyName)
                    .Select();

                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN Customers ON (Customers.CustomerID = Orders.CustomerID) ORDER BY Orders.OrderDate ASC, Customers.CompanyName ASC";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
                _connectionProvider.ResetCalls();

                // join with simple order by desc with generic then by
                context.From<Orders>().OrderByDesc(o => o.OrderDate).ThenBy<Orders>(o => o.RequiredDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ORDER BY Orders.OrderDate DESC, Orders.RequiredDate ASC";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
                _connectionProvider.ResetCalls();

                // join with generic order by desc with generic then by
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderByDesc<Orders>(o => o.OrderDate)
                    .ThenBy<Customers>(c => c.CompanyName)
                    .Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN Customers ON (Customers.CustomerID = Orders.CustomerID) ORDER BY Orders.OrderDate DESC, Customers.CompanyName ASC";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
                _connectionProvider.ResetCalls();

                // join with simple order by with generic then by desc
                context.From<Orders>().OrderBy(o => o.OrderDate).ThenByDesc<Orders>(o => o.RequiredDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ORDER BY Orders.OrderDate ASC, Orders.RequiredDate DESC";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
                _connectionProvider.ResetCalls();

                // join with generic order by with generic then by desc
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderBy<Orders>(o => o.OrderDate)
                    .ThenByDesc<Customers>(c => c.CompanyName)
                    .Select();

                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN Customers ON (Customers.CustomerID = Orders.CustomerID) ORDER BY Orders.OrderDate ASC, Customers.CompanyName DESC";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
                _connectionProvider.ResetCalls();

                // join with simple order by desc with generic then by desc
                context.From<Orders>().OrderByDesc(o => o.OrderDate).ThenByDesc<Orders>(o => o.RequiredDate).Select();

                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ORDER BY Orders.OrderDate DESC, Orders.RequiredDate DESC";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
                _connectionProvider.ResetCalls();

                // join with generic order by desc with generic then by desc
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderByDesc<Orders>(o => o.OrderDate)
                    .ThenByDesc<Customers>(c => c.CompanyName)
                    .Select();

                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN Customers ON (Customers.CustomerID = Orders.CustomerID) ORDER BY Orders.OrderDate DESC, Customers.CompanyName DESC";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Select_TestForOrders()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // select statement with a FOR expression and mapping members/fields to a specific table
                context.From<Orders>().Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID).For<Orders>().Map<Orders>(o => o.OrdersID).Select();

                var expected = "SELECT CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, Orders.OrdersID FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);

                // select statement with a FOR expression and ignoring fields in the resultset
                context.From<Orders>()
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)
                    .For<Orders>()
                    .Ignore(o => o.OrdersID)
                    .Ignore(o => o.OrderDate)
                    .Ignore(o => o.RequiredDate)
                    .Select();

                expected = "SELECT CustomerID, EmployeeID, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);

                // select statement that compiles FROM a FOR operation with a anonym object defining the resultset entries and mapped to a defined type
                context.From<Orders>().Map(o => o.OrdersID).Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID).For<Orders>().Select();
                expected = "SELECT Orders.OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);

                // simple select from statement
                context.From<Orders>().Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Select_TestWithForAndCustomMaps()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // select statement with a FOR expression and mapping members/fields to a specific table
                context.From<Orders>().Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID).For<Orders>().Map<Orders>(o => o.OrdersID).Select();

                var expected = "SELECT CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, Orders.OrdersID FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);

                // select statement with a FOR expression and ignoring fields in the resultset
                context.From<Warrior>().Join<Weapon>((wpn, wrir) => wpn.ID == wrir.WeaponID)
                    .For(() => new 
                    { 
                        WarriorName = "", 
                        WeaponName = "" 
                    })
                    .Map<Warrior>(wrir => wrir.Name, a => a.WarriorName)
                    .Map<Weapon>(wpn => wpn.Name, a => a.WeaponName)
                    .Select();

                expected = "SELECT Warrior.Name AS WarriorName, Weapon.Name AS WeaponName FROM Warrior JOIN Weapon ON (Weapon.ID = Warrior.WeaponID)";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Select_WithWhereTest()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // select statement with a where operation and a or operation that has two genereic parameters and alias for both types
                context.From<Customers>("cust")
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>((o, c) => o.EmployeeID == c.EmployeeID, source: "cust")
                    .Join<Employee>((e, o) => e.EmployeeID == o.EmployeeID, alias: "emp")
                    .Where(e => e.FirstName.Contains("Davolio"))
                    .Or<Customers, Employee>((c, e) => c.EmployeeID == e.EmployeeID, "cust", "emp")
                    .Select();

                var expected = "SELECT cust.EmployeeID, cust.Address, cust.City, cust.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo FROM Customers cust JOIN Orders ON (Orders.EmployeeID = cust.EmployeeID) JOIN Employee emp ON (emp.EmployeeID = Orders.EmployeeID) WHERE emp.FirstName like '%Davolio%' OR (cust.EmployeeID = emp.EmployeeID)";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);

                // select statement with a where operation and a or operation that has two genereic parameters and a alias ON the source type
                context.From<Customers>()
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>((o, c) => o.EmployeeID == c.EmployeeID)
                    .Join<Employee>((e, o) => e.EmployeeID == o.EmployeeID, alias: "emp")
                    .Where(e => e.FirstName.Contains("Davolio"))
                    .Or<Customers, Employee>((c, e) => c.EmployeeID == e.EmployeeID, source: "emp")
                    .Select();

                expected = "SELECT Customers.EmployeeID, Customers.Address, Customers.City, Customers.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo FROM Customers JOIN Orders ON (Orders.EmployeeID = Customers.EmployeeID) JOIN Employee emp ON (emp.EmployeeID = Orders.EmployeeID) WHERE emp.FirstName like '%Davolio%' OR (Customers.EmployeeID = emp.EmployeeID)";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);

                // select statement with a where operation and a or operation that has two genereic parameters and a alias ON the type
                context.From<Customers>("cust")
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>((o, c) => o.EmployeeID == c.EmployeeID, source: "cust")
                    .Join<Employee>((e, o) => e.EmployeeID == o.EmployeeID)
                    .Where(e => e.FirstName.Contains("Davolio"))
                    .Or<Customers, Employee>((c, e) => c.EmployeeID == e.EmployeeID, alias: "cust")
                    .Select();

                expected = "SELECT cust.EmployeeID, cust.Address, cust.City, cust.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo FROM Customers cust JOIN Orders ON (Orders.EmployeeID = cust.EmployeeID) JOIN Employee ON (Employee.EmployeeID = Orders.EmployeeID) WHERE Employee.FirstName like '%Davolio%' OR (cust.EmployeeID = Employee.EmployeeID)";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);

                // Select statement with a simple where operation
                context.From<Orders>()
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((d, o) => d.OrdersID == o.OrdersID)
                    .Where(o => o.Discount > 0)
                    .Rebase<OrderDetails, Orders>()
                    .Select();

                expected = "SELECT Orders.OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID) WHERE (OrderDetails.Discount > '0')";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);

                // select statement with a where and a simple or operation
                context.From<Orders>()
                    .Where(p => p.CustomerID.StartsWith("P"))
                    .Or<Orders>(o => o.ShipCity == "London")
                    .Select();

                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders WHERE Orders.CustomerID like 'P%' OR (Orders.ShipCity = 'London')";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);

                // select statement with a where and a simple or operation
                context.From<Orders>()
                    .Where(p => p.CustomerID.StartsWith("P"))
                    .Or<Orders>(o => o.ShipCity == "Paris")
                    .Or<Orders>(o => o.ShipCity == "London")
                    .Select();

                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders WHERE Orders.CustomerID like 'P%' OR (Orders.ShipCity = 'Paris') OR (Orders.ShipCity = 'London')";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);

                // Select statement with a where and a generic OR operation
                context.From<Orders>("ord")
                    .Where(p => p.CustomerID.StartsWith("P"))
                    .Or<Orders>(o => o.ShipCity == "London", "ord")
                    .Select();

                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ord WHERE ord.CustomerID like 'P%' OR (ord.ShipCity = 'London')";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);

                // select statement with a where and a simple and operation
                context.From<Orders>()
                    .Where(p => p.CustomerID.StartsWith("se"))
                    .And(o => o.ShipCity == "London")
                    .Select();

                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders WHERE Orders.CustomerID like 'se%' AND (Orders.ShipCity = 'London')";
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == expected)), Times.Once);

            }
        }

        [Test]
        public void ISelectQueryExpressionWithIgnoringFields()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // ignore a member in the select
                context.From<WarriorWithName>()
                    .Ignore(w => w.ID)
                    .Ignore(w => w.Name)
                    .Ignore(w => w.SpecialSkill)
                    .Select();

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT WeaponID, Race FROM WarriorWithName")), Times.Once);

                // ignore a member in the select
                context.From<WarriorWithName>()
                    .Ignore(w => w.ID)
                    .Ignore(w => w.Name)
                    .Ignore(w => w.SpecialSkill)
                    .Map(w => w.Name)
                    .Select();

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT WarriorWithName.Name, WeaponID, Race FROM WarriorWithName")), Times.Once);

                // ignore a member in the select
                context.From<WarriorWithName>()
                    .Ignore(w => w.ID)
                    .Ignore(w => w.Name)
                    .Map(w => w.WeaponID, "TestFieldName")
                    .Ignore(w => w.SpecialSkill)
                    .Select();

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT WarriorWithName.WeaponID AS TestFieldName, Race FROM WarriorWithName")), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Select_WithMultipleMapsToSameType()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // select the properties that are defined in the mapping
                context.From<WarriorWithName>()
                    .Map(w => w.WeaponID, "ID")
                    .Map(w => w.WeaponID)
                    .Map(w => w.Race, "Name")
                    .Map(w => w.Race)
                    .Select();

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT WarriorWithName.WeaponID AS ID, WarriorWithName.WeaponID, WarriorWithName.Race AS Name, WarriorWithName.Race, SpecialSkill FROM WarriorWithName")), Times.Once);

                // map one property to a custom field
                context.From<WarriorWithName>()
                    .Map(w => w.WeaponID, "ID")
                    .Map(w => w.Race, "Name")
                    .Map(w => w.Race)
                    .Select();

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT WarriorWithName.WeaponID AS ID, WarriorWithName.Race AS Name, WarriorWithName.Race, SpecialSkill FROM WarriorWithName")), Times.Once);
            }
        }

        [Test]
        [NUnit.Framework.Ignore("Not jet implemented")]
        public void PersistenceMap_Integration_Select_WithConstraintInBaseClass()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // select the properties that are defined in the mapping
                context.From<WarriorDerivate>(a => a.ID == 5).Select(() => new { ID = 0 });
                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT ID FROM WarriorDerivate WHERE (WarriorDerivate.ID = 5)")), Times.Once);
            }
        }
        
        [Test]
        public void PersistenceMap_Integration_Select_WithDifferenctCasesInMappedPropertyNamesTest()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.From<ArbeitsPlan>()
                        .Map(ap => ap.PlanID)
                        .Map<int>(ap => ap.Status, converter: value => value == 1 ? Status.Active : Status.Inactive)
                        .Map<ArbeitsPlanHeader>(ap => ap.PlanID, aph => aph.ID)
                        .Map<ArbeitsPlanHeader>(ap => ap.ATID, aph => aph.ArbeitsTageId)
                        .Map(ap => ap.Name)
                        .Join<Schemas>((s, ap) => s.SchemaID == ap.SchemaID)
                        .Map(s => s.SchemaID)
                        .Where(ap => ap.Status == 1)
                        .For<ArbeitsPlanHeader>()
                        .Ignore(aph => aph.Layout)
                        .Select();

                _connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s.Flatten() == "SELECT ArbeitsPlan.PlanID, ArbeitsPlan.Status, ArbeitsPlan.PlanID AS ID, ArbeitsPlan.ATID AS ArbeitsTageId, Schemas.SchemaID, JahrId, Von, Bis FROM ArbeitsPlan JOIN Schemas ON (Schemas.SchemaID = ArbeitsPlan.SchemaID) WHERE (Schemas.Status = 1)")), Times.Once);
            }
        }



        public class ArbeitsPlan
        {
            public int PlanID { get; set; }

            public int SchmaID { get; set; }

            public int ATID { get; set; }

            public string Name { get; set; }

            public int JahrID { get; set; }

            public int SchemaID { get; set; }

            public DateTime Von { get; set; }

            public DateTime Bis { get; set; }

            public int Status { get; set; }

            public string Mandid { get; set; }
        }

        class Schemas
        {
            public int SchemaID { get; set; }

            public string Name { get; set; }

            public int Status { get; set; }

            public string MandId { get; set; }
        }

        public class ArbeitsPlanHeader : ItemBase
        {
            public int PlanId { get; set; }
            public int ArbeitsTageId { get; set; }
            public int JahrId { get; set; }
            public int SchemaId { get; set; }
            public DateTime Von { get; set; }
            public DateTime Bis { get; set; }
            public Status Status { get; set; }
            public string Layout { get; set; }
        }

        public enum Status
        {
            Active,
            Inactive
        }
    }
}
