using NUnit.Framework;
using PersistenceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersistenceMap.Test.Expression
{
    [TestFixture]
    public class SelectExpressionTests
    {
        [Test]
        public void SelectWithPredicate()
        {
            var sql = string.Empty;
            var provider = new MockedContextProvider();
            provider.Interceptor<Warrior>().BeforeExecute(s => sql = s.QueryString.Flatten());
            using (var context = provider.Open())
            {
                // ignore a member in the select
                context.From<Warrior>(w => w.ID == 1)
                    .Select();

                Assert.AreEqual(sql, "SELECT ID, Name, WeaponID, Race, SpecialSkill FROM Warrior WHERE (Warrior.ID = 1)");

                // ignore a member in the SELECT
                context.Select<Warrior>(w => w.ID == 1);

                Assert.AreEqual(sql, "SELECT ID, Name, WeaponID, Race, SpecialSkill FROM Warrior WHERE (Warrior.ID = 1)");
            }
        }

        [Test]
        public void SelectWithGroupBy()
        {
            var sql = "";
            var provider = new MockedContextProvider(q => sql = q.Flatten());
            provider.Interceptor<Warrior>().BeforeExecute(q => sql = q.QueryString.Flatten());
            using (var context = provider.Open())
            {
                context.From<Warrior>().Ignore(w => w.ID).Ignore(w => w.SpecialSkill).Ignore(w => w.WeaponID).Ignore(w => w.Name).GroupBy(w => w.Race).Select();
                Assert.AreEqual(sql, "SELECT Race FROM Warrior GROUP BY Race");

                context.From<Warrior>().GroupBy(w => w.Race).ThenBy(w => w.WeaponID).For<Warrior>().Ignore(w => w.ID).Ignore(w => w.SpecialSkill).Ignore(w => w.Name).Select();
                Assert.AreEqual(sql, "SELECT WeaponID, Race FROM Warrior GROUP BY Race, WeaponID");

                context.From<Warrior>().For(() => new { ID = 0, Race = "" }).GroupBy(w => w.Race).ThenBy(w => w.ID).Select();
                Assert.AreEqual(sql, "SELECT ID, Race FROM Warrior GROUP BY Race, ID");

                context.From<Warrior>().Join<Weapon>((wep, war) => wep.ID == war.WeaponID).Where(w => w.Damage > 20).GroupBy<Warrior>(w => w.Race).For(() => new { Race = "" }).Select();
                Assert.AreEqual(sql, "SELECT Race FROM Warrior JOIN Weapon ON (Weapon.ID = Warrior.WeaponID) WHERE (Weapon.Damage > 20) GROUP BY Race");

                context.From<Warrior>().Join<Weapon>((wep, war) => wep.ID == war.WeaponID).Where(w => w.Damage > 20).GroupBy<Warrior>(w => w.Race).ThenBy<Warrior>(w => w.WeaponID).For(() => new { Race = "", WeaponID = 0 }).Select();
                Assert.AreEqual(sql, "SELECT Race, WeaponID FROM Warrior JOIN Weapon ON (Weapon.ID = Warrior.WeaponID) WHERE (Weapon.Damage > 20) GROUP BY Race, WeaponID");

                context.From<Warrior>().Join<Weapon>((wep, war) => wep.ID == war.WeaponID).Where(w => w.Damage > 20).For(() => new { Race = "" }).GroupBy(w => w.Race).Select();
                Assert.AreEqual(sql, "SELECT Race FROM Warrior JOIN Weapon ON (Weapon.ID = Warrior.WeaponID) WHERE (Weapon.Damage > 20) GROUP BY Race");

                context.From<Warrior>().Join<Weapon>((wep, war) => wep.ID == war.WeaponID).Where(w => w.Damage > 20).For(() => new { Race = "", WeaponID = 0 }).GroupBy(w => w.Race).ThenBy(w => w.WeaponID).Select();
                Assert.AreEqual(sql, "SELECT Race, WeaponID FROM Warrior JOIN Weapon ON (Weapon.ID = Warrior.WeaponID) WHERE (Weapon.Damage > 20) GROUP BY Race, WeaponID");
            }
        }

        [Test]
        public void SelectWithMax()
        {
            var sql = string.Empty;
            var provider = new MockedContextProvider(q => sql = q.Flatten());
            provider.Interceptor<Warrior>().BeforeExecute(q => sql = q.QueryString.Flatten());
            using (var context = provider.Open())
            {
                // select the max id
                context.From<Warrior>().Max(w => w.ID).Select();
                Assert.AreEqual(sql, "SELECT MAX(ID) AS ID FROM Warrior");

                // SELECT the max id with grouping
                context.From<Warrior>().Max(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).Select();
                Assert.AreEqual(sql, "SELECT MAX(ID) AS ID, Warrior.Race FROM Warrior GROUP BY Race");

                // select the max id with grouping
                context.From<Warrior>().Max(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).For<Warrior>().Select();
                Assert.AreEqual(sql, "SELECT MAX(ID) AS ID, Warrior.Race FROM Warrior GROUP BY Race");

                context.From<Warrior>().Max(w => w.ID, "MaxID").For(() => new { MaxID = 0 }).Select();
                Assert.AreEqual(sql, "SELECT MAX(ID) AS MaxID FROM Warrior");
            }
        }

        [Test]
        public void SelectWithMin()
        {
            var sql = "";
            var provider = new MockedContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // select the min id
                context.From<Warrior>().Min(w => w.ID).Select();
                Assert.AreEqual(sql, "SELECT MIN(ID) AS ID FROM Warrior");

                // select the min id with grouping
                context.From<Warrior>().Min(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).Select();
                Assert.AreEqual(sql, "SELECT MIN(ID) AS ID, Warrior.Race FROM Warrior GROUP BY Race");

                // select the min id with grouping
                context.From<Warrior>().Min(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).For<Warrior>().Select();
                Assert.AreEqual(sql, "SELECT MIN(ID) AS ID, Warrior.Race FROM Warrior GROUP BY Race");

                // select the min id
                context.From<Warrior>().Min(w => w.ID, "MinID").For(() => new { MinID = 0 }).Select();
                Assert.AreEqual(sql, "SELECT MIN(ID) AS MinID FROM Warrior");
            }
        }

        [Test]
        public void SelectWithCount()
        {
            var sql = "";
            var provider = new MockedContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // select the min id
                context.From<Warrior>().Count(w => w.ID).Select();
                Assert.AreEqual(sql, "SELECT COUNT(ID) AS ID FROM Warrior");

                // select the min id with grouping
                context.From<Warrior>().Count(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).Select();
                Assert.AreEqual(sql, "SELECT COUNT(ID) AS ID, Warrior.Race FROM Warrior GROUP BY Race");

                // select the min id with grouping
                context.From<Warrior>().Count(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).For<Warrior>().Select();
                Assert.AreEqual(sql, "SELECT COUNT(ID) AS ID, Warrior.Race FROM Warrior GROUP BY Race");

                // select the min id
                context.From<Warrior>().Count(w => w.ID, "IdCount").For(() => new { IdCount = 0 }).Select();
                Assert.AreEqual(sql, "SELECT COUNT(ID) AS IdCount FROM Warrior");
            }
        }

        [Test]
        public void SelectWithAliasMapping()
        {
            var expected = "SELECT Orders.Freight AS SpecialFreight, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new MockedContextProvider();
            provider.Interceptor<OrderWithDetailExtended>().BeforeExecute(s => Assert.AreEqual(s.QueryString.Flatten(), expected));
            using (var context = provider.Open())
            {
                // Map => To 
                context.From<Orders>()
                    .Map<OrderWithDetailExtended>(source => source.Freight, alias => alias.SpecialFreight)
                    .Join<OrderDetails>((detail, order) => detail.OrdersID == order.OrdersID)
                    // map a property from a JOIN to a property in the result type
                    .Map(i => i.OrdersID)
                    .Select<OrderWithDetailExtended>();
            }
        }

        [Test]
        public void SelectWithExtendedMapping()
        {
            var expected = "SELECT Orders.Freight AS SpecialFreight, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new MockedContextProvider();
            provider.Interceptor<OrderWithDetailExtended>().BeforeExecute(s => Assert.AreEqual(s.QueryString.Flatten(), expected));
            using (var context = provider.Open())
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
            var expected = "SELECT ProductID, Quantity FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";
            var type = new
            {
                ProductID = 0,
                Quantity = 0.0
            };

            var provider = new MockedContextProvider();
            provider.Interceptor(() => type).BeforeExecute(s => Assert.AreEqual(s.QueryString.Flatten(), expected));
            using (var context = provider.Open())
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
            var expected = "SELECT Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new MockedContextProvider(s => Assert.AreEqual(s.Flatten(), expected));
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
            }
        }

        [Test(Description = "Select to a anonym object delegate")]
        public void SelectAnonymObject2()
        {
            var expected = "SELECT Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new MockedContextProvider(s => Assert.AreEqual(s.Flatten(), expected));
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
            }
        }

        [Test(Description = "Select to a type object delegate")]
        public void SelectCustomObjectWithDelegate()
        {
            var expected = "SELECT Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new MockedContextProvider(s => Assert.AreEqual(s.Flatten(), expected));
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
            }
        }

        [Test]
        [Description("select statement that compiles from a FOR operation with a anonym object defining the resultset entries")]
        public void SelectForAnonymObjectType()
        {
            var expected = "SELECT ProductID, Quantity FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new MockedContextProvider(s => Assert.AreEqual(s.Flatten(), expected));
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
            }
        }

        [Test]
        public void SelectWithINExpression()
        {
            var expected = "SELECT ID, Name, WeaponID, Race, SpecialSkill FROM Warrior WHERE Warrior.Race In ('Elf','Dwarf')";

            var provider = new MockedContextProvider(s => Assert.AreEqual(s.Flatten(), expected));
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
            }
        }

        [Test]
        public void SelectWithOrderTest()
        {
            var sql = "";
            var provider = new MockedContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // join with simple order by
                context.From<Orders>().OrderBy(o => o.OrderDate).Select();
                string expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ORDER BY Orders.OrderDate ASC";
                Assert.AreEqual(sql, expected);

                // join with generic order by
                context.From<Orders>().Join<Customers>((c, o) => c.CustomerID == o.CustomerID).Map(c => c.CustomerID).Map(c => c.EmployeeID).OrderBy<Orders>(o => o.OrderDate).Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN Customers ON (Customers.CustomerID = Orders.CustomerID) ORDER BY Orders.OrderDate ASC";
                Assert.AreEqual(sql, expected);

                // join with simple order by desc
                context.From<Orders>().OrderByDesc(o => o.OrderDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ORDER BY Orders.OrderDate DESC";
                Assert.AreEqual(sql, expected);

                // join with generic order by desc
                context.From<Orders>().Join<Customers>((c, o) => c.CustomerID == o.CustomerID).Map(c => c.CustomerID).Map(c => c.EmployeeID).OrderByDesc<Orders>(o => o.OrderDate).Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN Customers ON (Customers.CustomerID = Orders.CustomerID) ORDER BY Orders.OrderDate DESC";
                Assert.AreEqual(sql, expected);

                // join with simple order by with simple then by
                context.From<Orders>().OrderBy(o => o.OrderDate).ThenBy(o => o.RequiredDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ORDER BY Orders.OrderDate ASC, Orders.RequiredDate ASC";
                Assert.AreEqual(sql, expected);

                // join with generic order by with simple then by
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderBy<Orders>(o => o.OrderDate)
                    .ThenBy(o => o.RequiredDate)
                    .Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN Customers ON (Customers.CustomerID = Orders.CustomerID) ORDER BY Orders.OrderDate ASC, Orders.RequiredDate ASC";
                Assert.AreEqual(sql, expected);

                // join with simple order by desc with simple then by
                context.From<Orders>().OrderByDesc(o => o.OrderDate).ThenBy(o => o.RequiredDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ORDER BY Orders.OrderDate DESC, Orders.RequiredDate ASC";
                Assert.AreEqual(sql, expected);

                // join with generic order by desc with simple then by
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderByDesc<Orders>(o => o.OrderDate)
                    .ThenBy(o => o.RequiredDate)
                    .Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN Customers ON (Customers.CustomerID = Orders.CustomerID) ORDER BY Orders.OrderDate DESC, Orders.RequiredDate ASC";
                Assert.AreEqual(sql, expected);

                // join with simple order by with generic then by
                context.From<Orders>().OrderBy(o => o.OrderDate).ThenBy<Orders>(o => o.RequiredDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ORDER BY Orders.OrderDate ASC, Orders.RequiredDate ASC";
                Assert.AreEqual(sql, expected);

                // join with generic order by with generic then by
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderBy<Orders>(o => o.OrderDate)
                    .ThenBy<Customers>(c => c.CompanyName)
                    .Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN Customers ON (Customers.CustomerID = Orders.CustomerID) ORDER BY Orders.OrderDate ASC, Customers.CompanyName ASC";
                Assert.AreEqual(sql, expected);

                // join with simple order by desc with generic then by
                context.From<Orders>().OrderByDesc(o => o.OrderDate).ThenBy<Orders>(o => o.RequiredDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ORDER BY Orders.OrderDate DESC, Orders.RequiredDate ASC";
                Assert.AreEqual(sql, expected);

                // join with generic order by desc with generic then by
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderByDesc<Orders>(o => o.OrderDate)
                    .ThenBy<Customers>(c => c.CompanyName)
                    .Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN Customers ON (Customers.CustomerID = Orders.CustomerID) ORDER BY Orders.OrderDate DESC, Customers.CompanyName ASC";
                Assert.AreEqual(sql, expected);

                // join with simple order by with generic then by desc
                context.From<Orders>().OrderBy(o => o.OrderDate).ThenByDesc<Orders>(o => o.RequiredDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ORDER BY Orders.OrderDate ASC, Orders.RequiredDate DESC";
                Assert.AreEqual(sql, expected);

                // join with generic order by with generic then by desc
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderBy<Orders>(o => o.OrderDate)
                    .ThenByDesc<Customers>(c => c.CompanyName)
                    .Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN Customers ON (Customers.CustomerID = Orders.CustomerID) ORDER BY Orders.OrderDate ASC, Customers.CompanyName DESC";
                Assert.AreEqual(sql, expected);

                // join with simple order by desc with generic then by desc
                context.From<Orders>().OrderByDesc(o => o.OrderDate).ThenByDesc<Orders>(o => o.RequiredDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ORDER BY Orders.OrderDate DESC, Orders.RequiredDate DESC";
                Assert.AreEqual(sql, expected);

                // join with generic order by desc with generic then by desc
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderByDesc<Orders>(o => o.OrderDate)
                    .ThenByDesc<Customers>(c => c.CompanyName)
                    .Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN Customers ON (Customers.CustomerID = Orders.CustomerID) ORDER BY Orders.OrderDate DESC, Customers.CompanyName DESC";
                Assert.AreEqual(sql, expected);
            }
        }

        [Test]
        public void SelectTestForOrders()
        {
            var sql = "";
            var provider = new MockedContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // select statement with a FOR expression and mapping members/fields to a specific table
                context.From<Orders>().Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID).For<Orders>().Map<Orders>(o => o.OrdersID).Select();
                var expected = "SELECT CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, Orders.OrdersID FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";
                Assert.AreEqual(sql, expected);

                // select statement with a FOR expression and ignoring fields in the resultset
                context.From<Orders>()
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)
                    .For<Orders>()
                    .Ignore(o => o.OrdersID)
                    .Ignore(o => o.OrderDate)
                    .Ignore(o => o.RequiredDate)
                    .Select();
                expected = "SELECT CustomerID, EmployeeID, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";
                Assert.AreEqual(sql, expected);

                // select statement that compiles FROM a FOR operation with a anonym object defining the resultset entries and mapped to a defined type
                context.From<Orders>().Map(o => o.OrdersID).Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID).For<Orders>().Select();
                expected = "SELECT Orders.OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";
                Assert.AreEqual(sql, expected);

                // simple select from statement
                context.From<Orders>().Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders";
                Assert.AreEqual(sql, expected);
            }
        }

        [Test]
        public void SelectTestWithForAndCustomMaps()
        {
            var sql = "";
            var provider = new MockedContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // select statement with a FOR expression and mapping members/fields to a specific table
                context.From<Orders>().Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID).For<Orders>().Map<Orders>(o => o.OrdersID).Select();
                var expected = "SELECT CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, Orders.OrdersID FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID)";
                Assert.AreEqual(sql, expected);

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
                Assert.AreEqual(sql, expected);
            }
        }

        [Test]
        public void SelectWithWhereTest()
        {
            var sql = "";
            var provider = new MockedContextProvider(s => sql = s.Flatten());
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
                    //"select cust.EmployeeID, OrdersID, CustomerID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Customers cust JOIN Orders ON (Orders.EmployeeID = cust.EmployeeID) JOIN Employee emp ON (emp.EmployeeID = Orders.EmployeeID) where emp.FirstName like '%Davolio%' or (cust.EmployeeID = emp.EmployeeID)";
                Assert.AreEqual(sql, expected);

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
                //expected = "select Customers.EmployeeID, OrdersID, CustomerID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Customers JOIN Orders ON (Orders.EmployeeID = Customers.EmployeeID) JOIN Employee emp ON (emp.EmployeeID = Orders.EmployeeID) where emp.FirstName like '%Davolio%' or (Customers.EmployeeID = emp.EmployeeID)";
                Assert.AreEqual(sql, expected);

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
                //expected = "select cust.EmployeeID, OrdersID, CustomerID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Customers cust JOIN Orders ON (Orders.EmployeeID = cust.EmployeeID) JOIN Employee ON (Employee.EmployeeID = Orders.EmployeeID) where Employee.FirstName like '%Davolio%' or (cust.EmployeeID = Employee.EmployeeID)";
                Assert.AreEqual(sql, expected);

                // Select statement with a simple where operation
                context.From<Orders>()
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((d, o) => d.OrdersID == o.OrdersID)
                    .Where(o => o.Discount > 0)
                    .Rebase<OrderDetails, Orders>()
                    .Select();
                expected = "SELECT Orders.OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders JOIN OrderDetails ON (OrderDetails.OrdersID = Orders.OrdersID) WHERE (OrderDetails.Discount > '0')";
                Assert.AreEqual(sql, expected);

                // select statement with a where and a simple or operation
                context.From<Orders>()
                    .Where(p => p.CustomerID.StartsWith("P"))
                    .Or<Orders>(o => o.ShipCity == "London")
                    .Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders WHERE Orders.CustomerID like 'P%' OR (Orders.ShipCity = 'London')";
                Assert.AreEqual(sql, expected);

                // select statement with a where and a simple or operation
                context.From<Orders>()
                    .Where(p => p.CustomerID.StartsWith("P"))
                    .Or<Orders>(o => o.ShipCity == "Paris")
                    .Or<Orders>(o => o.ShipCity == "London")
                    .Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders WHERE Orders.CustomerID like 'P%' OR (Orders.ShipCity = 'Paris') OR (Orders.ShipCity = 'London')";
                Assert.AreEqual(sql, expected);

                // Select statement with a where and a generic OR operation
                context.From<Orders>("ord")
                    .Where(p => p.CustomerID.StartsWith("P"))
                    .Or<Orders>(o => o.ShipCity == "London", "ord")
                    .Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders ord WHERE ord.CustomerID like 'P%' OR (ord.ShipCity = 'London')";
                Assert.AreEqual(sql, expected);

                // select statement with a where and a simple and operation
                context.From<Orders>()
                    .Where(p => p.CustomerID.StartsWith("se"))
                    .And(o => o.ShipCity == "London")
                    .Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM Orders WHERE Orders.CustomerID like 'se%' AND (Orders.ShipCity = 'London')";
                Assert.AreEqual(sql, expected);

            }
        }

        [Test]
        public void ISelectQueryExpressionWithIgnoringFields()
        {
            var sql = "";
            var provider = new MockedContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // ignore a member in the select
                context.From<WarriorWithName>()
                    .Ignore(w => w.ID)
                    .Ignore(w => w.Name)
                    .Ignore(w => w.SpecialSkill)
                    .Select();

                Assert.AreEqual(sql, "SELECT WeaponID, Race FROM WarriorWithName");

                // ignore a member in the select
                context.From<WarriorWithName>()
                    .Ignore(w => w.ID)
                    .Ignore(w => w.Name)
                    .Ignore(w => w.SpecialSkill)
                    .Map(w => w.Name)
                    .Select();

                Assert.AreEqual(sql, "SELECT WarriorWithName.Name, WeaponID, Race FROM WarriorWithName");

                // ignore a member in the select
                context.From<WarriorWithName>()
                    .Ignore(w => w.ID)
                    .Ignore(w => w.Name)
                    .Map(w => w.WeaponID, "TestFieldName")
                    .Ignore(w => w.SpecialSkill)
                    .Select();

                Assert.AreEqual(sql, "SELECT WarriorWithName.WeaponID AS TestFieldName, Race FROM WarriorWithName");
            }
        }

        [Test]
        public void SelectWithMultipleMapsToSameType()
        {
            var sql = "";
            var provider = new MockedContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // select the properties that are defined in the mapping
                context.From<WarriorWithName>()
                    .Map(w => w.WeaponID, "ID")
                    .Map(w => w.WeaponID)
                    .Map(w => w.Race, "Name")
                    .Map(w => w.Race)
                    .Select();

                Assert.AreEqual(sql, "SELECT WarriorWithName.WeaponID AS ID, WarriorWithName.WeaponID, WarriorWithName.Race AS Name, WarriorWithName.Race, SpecialSkill FROM WarriorWithName");

                // map one property to a custom field
                context.From<WarriorWithName>()
                    .Map(w => w.WeaponID, "ID")
                    .Map(w => w.Race, "Name")
                    .Map(w => w.Race)
                    .Select();

                Assert.AreEqual(sql, "SELECT WarriorWithName.WeaponID AS ID, WarriorWithName.Race AS Name, WarriorWithName.Race, SpecialSkill FROM WarriorWithName");
            }
        }

        [Test]
        [NUnit.Framework.Ignore("Not jet implemented")]
        public void SelectWithConstraintInBaseClass()
        {
            var sql = "";
            var provider = new MockedContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // select the properties that are defined in the mapping
                context.From<WarriorDerivate>(a => a.ID == 5).Select(() => new { ID = 0 }).FirstOrDefault();
                Assert.AreEqual(sql, "SELECT ID FROM WarriorDerivate WHERE (WarriorDerivate.ID = 5)");
            }
        }

        //[Test]
        //public void SelectWithMapAndValueConverter()
        //{
        //    var sql = "";
        //    var provider = new CallbackContextProvider(s => sql = s.Flatten());
        //    using (var context = provider.Open())
        //    {
        //        // select the max id with grouping
        //        context.From<Warrior>().Map(w => w.Race, "ID", opt => opt.ConvertValue<int>(id => id > 0 ? "ID is greader than 0" : "ID is smaller than 0")).GroupBy(w => w.Race).Select();
        //        Assert.AreEqual(sql, "SELECT ID AS Race, Warrior.Race from Warrior GROUP BY Race");
        //    }
        //}

        [Test]
        public void SelectWithDifferenctCasesInMappedPropertyNamesTest()
        {
            var sql = "";
            var provider = new MockedContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                var query = context.From<ArbeitsPlan>()
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

                Assert.AreEqual(sql, "SELECT ArbeitsPlan.PlanID, ArbeitsPlan.Status, ArbeitsPlan.PlanID AS ID, ArbeitsPlan.ATID AS ArbeitsTageId, Schemas.SchemaID, JahrId, Von, Bis FROM ArbeitsPlan JOIN Schemas ON (Schemas.SchemaID = ArbeitsPlan.SchemaID) WHERE (Schemas.Status = 1)");
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
