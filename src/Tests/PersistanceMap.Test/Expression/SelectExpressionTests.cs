using NUnit.Framework;
using PersistanceMap.Test.TableTypes;
using System.Collections.Generic;
using System.Linq;

namespace PersistanceMap.Test.Expression
{
    [TestFixture]
    public class SelectExpressionTests
    {
        [Test]
        public void SelectWithPredicate()
        {
            var sql = "";
            var provider = new CallbackContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // ignore a member in the select
                context.From<Warrior>(w => w.ID == 1)
                    .Select();

                Assert.AreEqual(sql, "SELECT ID, Name, WeaponID, Race, SpecialSkill from Warrior where (Warrior.ID = 1)");

                // ignore a member in the SELECT
                context.Select<Warrior>(w => w.ID == 1);

                Assert.AreEqual(sql, "SELECT ID, Name, WeaponID, Race, SpecialSkill from Warrior where (Warrior.ID = 1)");
            }
        }

        [Test]
        public void SelectWithGroupBy()
        {
            var sql = "";
            var provider = new CallbackContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                context.From<Warrior>().Ignore(w => w.ID).Ignore(w => w.SpecialSkill).Ignore(w => w.WeaponID).Ignore(w => w.Name).GroupBy(w => w.Race).Select();
                Assert.AreEqual(sql, "SELECT Race from Warrior GROUP BY Race");

                context.From<Warrior>().GroupBy(w => w.Race).ThenBy(w => w.WeaponID).For<Warrior>().Ignore(w => w.ID).Ignore(w => w.SpecialSkill).Ignore(w => w.Name).Select();
                Assert.AreEqual(sql, "SELECT WeaponID, Race from Warrior GROUP BY Race, WeaponID");

                context.From<Warrior>().For(() => new { ID = 0, Race = "" }).GroupBy(w => w.Race).ThenBy(w => w.ID).Select();
                Assert.AreEqual(sql, "SELECT ID, Race from Warrior GROUP BY Race, ID");

                context.From<Warrior>().Join<Weapon>((wep, war) => wep.ID == war.WeaponID).Where(w => w.Damage > 20).GroupBy<Warrior>(w => w.Race).For(() => new { Race = "" }).Select();
                Assert.AreEqual(sql, "SELECT Race from Warrior join Weapon on (Weapon.ID = Warrior.WeaponID) where (Weapon.Damage > 20) GROUP BY Race");

                context.From<Warrior>().Join<Weapon>((wep, war) => wep.ID == war.WeaponID).Where(w => w.Damage > 20).GroupBy<Warrior>(w => w.Race).ThenBy<Warrior>(w => w.WeaponID).For(() => new { Race = "", WeaponID = 0 }).Select();
                Assert.AreEqual(sql, "SELECT Race, WeaponID from Warrior join Weapon on (Weapon.ID = Warrior.WeaponID) where (Weapon.Damage > 20) GROUP BY Race, WeaponID");

                context.From<Warrior>().Join<Weapon>((wep, war) => wep.ID == war.WeaponID).Where(w => w.Damage > 20).For(() => new { Race = "" }).GroupBy(w => w.Race).Select();
                Assert.AreEqual(sql, "SELECT Race from Warrior join Weapon on (Weapon.ID = Warrior.WeaponID) where (Weapon.Damage > 20) GROUP BY Race");

                context.From<Warrior>().Join<Weapon>((wep, war) => wep.ID == war.WeaponID).Where(w => w.Damage > 20).For(() => new { Race = "", WeaponID = 0 }).GroupBy(w => w.Race).ThenBy(w => w.WeaponID).Select();
                Assert.AreEqual(sql, "SELECT Race, WeaponID from Warrior join Weapon on (Weapon.ID = Warrior.WeaponID) where (Weapon.Damage > 20) GROUP BY Race, WeaponID");
            }
        }

        [Test]
        public void SelectWithMax()
        {
            var sql = "";
            var provider = new CallbackContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // select the max id
                context.From<Warrior>().Max(w => w.ID).Select();
                Assert.AreEqual(sql, "SELECT MAX(ID) AS ID from Warrior");

                // SELECT the max id with grouping
                context.From<Warrior>().Max(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).Select();
                Assert.AreEqual(sql, "SELECT MAX(ID) AS ID, Warrior.Race from Warrior GROUP BY Race");

                // select the max id with grouping
                context.From<Warrior>().Max(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).For<Warrior>().Select();
                Assert.AreEqual(sql, "SELECT MAX(ID) AS ID, Warrior.Race from Warrior GROUP BY Race");

                context.From<Warrior>().Max(w => w.ID, "MaxID").For(() => new { MaxID = 0 }).Select();
                Assert.AreEqual(sql, "SELECT MAX(ID) AS MaxID from Warrior");
            }
        }

        [Test]
        public void SelectWithMin()
        {
            var sql = "";
            var provider = new CallbackContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // select the min id
                context.From<Warrior>().Min(w => w.ID).Select();
                Assert.AreEqual(sql, "SELECT MIN(ID) AS ID from Warrior");

                // select the min id with grouping
                context.From<Warrior>().Min(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).Select();
                Assert.AreEqual(sql, "SELECT MIN(ID) AS ID, Warrior.Race from Warrior GROUP BY Race");

                // select the min id with grouping
                context.From<Warrior>().Min(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).For<Warrior>().Select();
                Assert.AreEqual(sql, "SELECT MIN(ID) AS ID, Warrior.Race from Warrior GROUP BY Race");

                // select the min id
                context.From<Warrior>().Min(w => w.ID, "MinID").For(() => new { MinID = 0 }).Select();
                Assert.AreEqual(sql, "SELECT MIN(ID) AS MinID from Warrior");
            }
        }

        [Test]
        public void SelectWithCount()
        {
            var sql = "";
            var provider = new CallbackContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // select the min id
                context.From<Warrior>().Count(w => w.ID).Select();
                Assert.AreEqual(sql, "SELECT COUNT(ID) AS ID from Warrior");

                // select the min id with grouping
                context.From<Warrior>().Count(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).Select();
                Assert.AreEqual(sql, "SELECT COUNT(ID) AS ID, Warrior.Race from Warrior GROUP BY Race");

                // select the min id with grouping
                context.From<Warrior>().Count(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).For<Warrior>().Select();
                Assert.AreEqual(sql, "SELECT COUNT(ID) AS ID, Warrior.Race from Warrior GROUP BY Race");

                // select the min id
                context.From<Warrior>().Count(w => w.ID, "IdCount").For(() => new { IdCount = 0 }).Select();
                Assert.AreEqual(sql, "SELECT COUNT(ID) AS IdCount from Warrior");
            }
        }

        [Test]
        public void SelectWithAliasMapping()
        {
            var expected = "SELECT Orders.Freight as SpecialFreight, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected));
            using (var context = provider.Open())
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
            var expected = "SELECT Orders.Freight as SpecialFreight, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected));
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
            var expected = "SELECT ProductID, Quantity from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected));
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
            var expected = "SELECT Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected));
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
            var expected = "SELECT Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected));
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
            var expected = "SELECT Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected));
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
            var expected = "SELECT ProductID, Quantity from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected));
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
            var expected = "SELECT ID, Name, WeaponID, Race, SpecialSkill from Warrior where Warrior.Race In ('Elf','Dwarf')";

            var provider = new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected));
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
            var provider = new CallbackContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // join with simple order by
                context.From<Orders>().OrderBy(o => o.OrderDate).Select();
                string expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate asc";
                Assert.AreEqual(sql, expected);

                // join with generic order by
                context.From<Orders>().Join<Customers>((c, o) => c.CustomerID == o.CustomerID).Map(c => c.CustomerID).Map(c => c.EmployeeID).OrderBy<Orders>(o => o.OrderDate).Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate asc";
                Assert.AreEqual(sql, expected);

                // join with simple order by desc
                context.From<Orders>().OrderByDesc(o => o.OrderDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate desc";
                Assert.AreEqual(sql, expected);

                // join with generic order by desc
                context.From<Orders>().Join<Customers>((c, o) => c.CustomerID == o.CustomerID).Map(c => c.CustomerID).Map(c => c.EmployeeID).OrderByDesc<Orders>(o => o.OrderDate).Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate desc";
                Assert.AreEqual(sql, expected);

                // join with simple order by with simple then by
                context.From<Orders>().OrderBy(o => o.OrderDate).ThenBy(o => o.RequiredDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate asc , Orders.RequiredDate ASC";
                Assert.AreEqual(sql, expected);

                // join with generic order by with simple then by
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderBy<Orders>(o => o.OrderDate)
                    .ThenBy(o => o.RequiredDate)
                    .Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate asc , Orders.RequiredDate ASC";
                Assert.AreEqual(sql, expected);

                // join with simple order by desc with simple then by
                context.From<Orders>().OrderByDesc(o => o.OrderDate).ThenBy(o => o.RequiredDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate desc , Orders.RequiredDate ASC";
                Assert.AreEqual(sql, expected);

                // join with generic order by desc with simple then by
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderByDesc<Orders>(o => o.OrderDate)
                    .ThenBy(o => o.RequiredDate)
                    .Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate desc , Orders.RequiredDate ASC";
                Assert.AreEqual(sql, expected);

                // join with simple order by with generic then by
                context.From<Orders>().OrderBy(o => o.OrderDate).ThenBy<Orders>(o => o.RequiredDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate asc , Orders.RequiredDate ASC";
                Assert.AreEqual(sql, expected);

                // join with generic order by with generic then by
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderBy<Orders>(o => o.OrderDate)
                    .ThenBy<Customers>(c => c.CompanyName)
                    .Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate asc , Customers.CompanyName ASC";
                Assert.AreEqual(sql, expected);

                // join with simple order by desc with generic then by
                context.From<Orders>().OrderByDesc(o => o.OrderDate).ThenBy<Orders>(o => o.RequiredDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate desc , Orders.RequiredDate ASC";
                Assert.AreEqual(sql, expected);

                // join with generic order by desc with generic then by
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderByDesc<Orders>(o => o.OrderDate)
                    .ThenBy<Customers>(c => c.CompanyName)
                    .Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate desc , Customers.CompanyName ASC";
                Assert.AreEqual(sql, expected);

                // join with simple order by with generic then by desc
                context.From<Orders>().OrderBy(o => o.OrderDate).ThenByDesc<Orders>(o => o.RequiredDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate asc , Orders.RequiredDate DESC";
                Assert.AreEqual(sql, expected);

                // join with generic order by with generic then by desc
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderBy<Orders>(o => o.OrderDate)
                    .ThenByDesc<Customers>(c => c.CompanyName)
                    .Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate asc , Customers.CompanyName DESC";
                Assert.AreEqual(sql, expected);

                // join with simple order by desc with generic then by desc
                context.From<Orders>().OrderByDesc(o => o.OrderDate).ThenByDesc<Orders>(o => o.RequiredDate).Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate desc , Orders.RequiredDate DESC";
                Assert.AreEqual(sql, expected);

                // join with generic order by desc with generic then by desc
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderByDesc<Orders>(o => o.OrderDate)
                    .ThenByDesc<Customers>(c => c.CompanyName)
                    .Select();
                expected = "SELECT Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate desc , Customers.CompanyName DESC";
                Assert.AreEqual(sql, expected);
            }
        }

        [Test]
        public void SelectTestForOrders()
        {
            var sql = "";
            var provider = new CallbackContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // select statement with a FOR expression and mapping members/fields to a specific table
                context.From<Orders>().Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID).For<Orders>().Map<Orders>(o => o.OrdersID).Select();
                var expected = "SELECT Orders.OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";
                Assert.AreEqual(sql, expected);

                // select statement with a FOR expression and ignoring fields in the resultset
                context.From<Orders>()
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)
                    .For<Orders>()
                    .Ignore(o => o.OrdersID)
                    .Ignore(o => o.OrderDate)
                    .Ignore(o => o.RequiredDate)
                    .Select();
                expected = "SELECT CustomerID, EmployeeID, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";
                Assert.AreEqual(sql, expected);

                // select statement that compiles from a FOR operation with a anonym object defining the resultset entries and mapped to a defined type
                context.From<Orders>().Map(o => o.OrdersID).Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID).For<Orders>().Select();
                expected = "SELECT Orders.OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";
                Assert.AreEqual(sql, expected);

                // simple select from statement
                context.From<Orders>().Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders";
                Assert.AreEqual(sql, expected);
            }
        }

        [Test]
        public void SelectWithWhereTest()
        {
            var sql = "";
            var provider = new CallbackContextProvider(s => sql = s.Flatten());
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
                var expected = "SELECT cust.EmployeeID, cust.Address, cust.City, cust.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Customers cust join Orders on (Orders.EmployeeID = cust.EmployeeID) join Employee emp on (emp.EmployeeID = Orders.EmployeeID) where emp.FirstName like '%Davolio%' or (cust.EmployeeID = emp.EmployeeID)";
                    //"select cust.EmployeeID, OrdersID, CustomerID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Customers cust join Orders on (Orders.EmployeeID = cust.EmployeeID) join Employee emp on (emp.EmployeeID = Orders.EmployeeID) where emp.FirstName like '%Davolio%' or (cust.EmployeeID = emp.EmployeeID)";
                Assert.AreEqual(sql, expected);

                // select statement with a where operation and a or operation that has two genereic parameters and a alias on the source type
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
                expected = "SELECT Customers.EmployeeID, Customers.Address, Customers.City, Customers.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Customers join Orders on (Orders.EmployeeID = Customers.EmployeeID) join Employee emp on (emp.EmployeeID = Orders.EmployeeID) where emp.FirstName like '%Davolio%' or (Customers.EmployeeID = emp.EmployeeID)";
                //expected = "select Customers.EmployeeID, OrdersID, CustomerID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Customers join Orders on (Orders.EmployeeID = Customers.EmployeeID) join Employee emp on (emp.EmployeeID = Orders.EmployeeID) where emp.FirstName like '%Davolio%' or (Customers.EmployeeID = emp.EmployeeID)";
                Assert.AreEqual(sql, expected);

                // select statement with a where operation and a or operation that has two genereic parameters and a alias on the type
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
                expected = "SELECT cust.EmployeeID, cust.Address, cust.City, cust.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Customers cust join Orders on (Orders.EmployeeID = cust.EmployeeID) join Employee on (Employee.EmployeeID = Orders.EmployeeID) where Employee.FirstName like '%Davolio%' or (cust.EmployeeID = Employee.EmployeeID)";
                //expected = "select cust.EmployeeID, OrdersID, CustomerID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Customers cust join Orders on (Orders.EmployeeID = cust.EmployeeID) join Employee on (Employee.EmployeeID = Orders.EmployeeID) where Employee.FirstName like '%Davolio%' or (cust.EmployeeID = Employee.EmployeeID)";
                Assert.AreEqual(sql, expected);

                // Select statement with a simple where operation
                context.From<Orders>()
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((d, o) => d.OrdersID == o.OrdersID)
                    .Where(o => o.Discount > 0)
                    .Rebase<OrderDetails, Orders>()
                    .Select();
                expected = "SELECT Orders.OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID) where (OrderDetails.Discount > '0')";
                Assert.AreEqual(sql, expected);

                // select statement with a where and a simple or operation
                context.From<Orders>()
                    .Where(p => p.CustomerID.StartsWith("P"))
                    .Or<Orders>(o => o.ShipCity == "London")
                    .Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders where Orders.CustomerID like 'P%' or (Orders.ShipCity = 'London')";
                Assert.AreEqual(sql, expected);

                // select statement with a where and a simple or operation
                context.From<Orders>()
                    .Where(p => p.CustomerID.StartsWith("P"))
                    .Or<Orders>(o => o.ShipCity == "Paris")
                    .Or<Orders>(o => o.ShipCity == "London")
                    .Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders where Orders.CustomerID like 'P%' or (Orders.ShipCity = 'Paris') or (Orders.ShipCity = 'London')";
                Assert.AreEqual(sql, expected);

                // Select statement with a where and a generic OR operation
                context.From<Orders>("ord")
                    .Where(p => p.CustomerID.StartsWith("P"))
                    .Or<Orders>(o => o.ShipCity == "London", "ord")
                    .Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders ord where ord.CustomerID like 'P%' or (ord.ShipCity = 'London')";
                Assert.AreEqual(sql, expected);

                // select statement with a where and a simple and operation
                context.From<Orders>()
                    .Where(p => p.CustomerID.StartsWith("se"))
                    .And(o => o.ShipCity == "London")
                    .Select();
                expected = "SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders where Orders.CustomerID like 'se%' and (Orders.ShipCity = 'London')";
                Assert.AreEqual(sql, expected);

            }
        }

        [Test]
        public void ISelectQueryExpressionWithIgnoringFields()
        {
            var sql = "";
            var provider = new CallbackContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // ignore a member in the select
                context.From<WarriorWithName>()
                    .Ignore(w => w.ID)
                    .Ignore(w => w.Name)
                    .Ignore(w => w.SpecialSkill)
                    .Select();

                Assert.AreEqual(sql, "SELECT WeaponID, Race from WarriorWithName");

                // ignore a member in the select
                context.From<WarriorWithName>()
                    .Ignore(w => w.ID)
                    .Ignore(w => w.Name)
                    .Ignore(w => w.SpecialSkill)
                    .Map(w => w.Name)
                    .Select();

                Assert.AreEqual(sql, "SELECT WarriorWithName.Name, WeaponID, Race from WarriorWithName");

                // ignore a member in the select
                context.From<WarriorWithName>()
                    .Ignore(w => w.ID)
                    .Ignore(w => w.Name)
                    .Map(w => w.WeaponID, "TestFieldName")
                    .Ignore(w => w.SpecialSkill)
                    .Select();

                Assert.AreEqual(sql, "SELECT WarriorWithName.WeaponID as TestFieldName, Race from WarriorWithName");
            }
        }

        [Test]
        public void SelectWithMultipleMapsToSameType()
        {
            var sql = "";
            var provider = new CallbackContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // select the properties that are defined in the mapping
                context.From<WarriorWithName>()
                    .Map(w => w.WeaponID, "ID")
                    .Map(w => w.WeaponID)
                    .Map(w => w.Race, "Name")
                    .Map(w => w.Race)
                    .Select();

                Assert.AreEqual(sql, "SELECT WarriorWithName.WeaponID as ID, WarriorWithName.WeaponID, WarriorWithName.Race as Name, WarriorWithName.Race, SpecialSkill from WarriorWithName");

                // map one property to a custom field
                context.From<WarriorWithName>()
                    .Map(w => w.WeaponID, "ID")
                    .Map(w => w.Race, "Name")
                    .Map(w => w.Race)
                    .Select();

                Assert.AreEqual(sql, "SELECT WarriorWithName.WeaponID as ID, WarriorWithName.Race as Name, WarriorWithName.Race, SpecialSkill from WarriorWithName");
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
    }
}
