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
        public void SelectWithPredicate()
        {
            var sql = "";
            var provider = new CallbackContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                // ignore a member in the select
                context.From<Warrior>(w => w.ID == 1)
                    .Select();

                Assert.AreEqual(sql, "select ID, WeaponID, Race, SpecialSkill from Warrior where (Warrior.ID = 1)");

                // ignore a member in the select
                context.Select<Warrior>(w => w.ID == 1);

                Assert.AreEqual(sql, "select ID, WeaponID, Race, SpecialSkill from Warrior where (Warrior.ID = 1)");
            }
        }

        [Test]
        public void SelectWithGroupBy()
        {
            var sql = "";
            var provider = new CallbackContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                context.From<Warrior>().Ignore(w => w.ID).Ignore(w => w.SpecialSkill).Ignore(w => w.WeaponID).GroupBy(w => w.Race).Select();
                Assert.AreEqual(sql, "select Race from Warrior GROUP BY Race");

                context.From<Warrior>().GroupBy(w => w.Race).ThenBy(w => w.WeaponID).For<Warrior>().Ignore(w => w.ID).Ignore(w => w.SpecialSkill).Select();
                Assert.AreEqual(sql, "select WeaponID, Race from Warrior GROUP BY Race, WeaponID");

                context.From<Warrior>().For(() => new { ID = 0, Race = "" }).GroupBy(w => w.Race).ThenBy(w => w.ID).Select();
                Assert.AreEqual(sql, "select ID, Race from Warrior GROUP BY Race, ID");

                context.From<Warrior>().Join<Weapon>((wep, war) => wep.WeaponID == war.WeaponID).Where(w => w.Damage > 20).GroupBy<Warrior>(w => w.Race).For(() => new { Race = "" }).Select();
                Assert.AreEqual(sql, "select Race from Warrior join Weapon on (Weapon.WeaponID = Warrior.WeaponID) where (Weapon.Damage > 20) GROUP BY Race");

                context.From<Warrior>().Join<Weapon>((wep, war) => wep.WeaponID == war.WeaponID).Where(w => w.Damage > 20).GroupBy<Warrior>(w => w.Race).ThenBy<Weapon>(w => w.WeaponID).For(() => new { Race = "", WeaponID = 0 }).Select();
                Assert.AreEqual(sql, "select Race, WeaponID from Warrior join Weapon on (Weapon.WeaponID = Warrior.WeaponID) where (Weapon.Damage > 20) GROUP BY Race, WeaponID");

                context.From<Warrior>().Join<Weapon>((wep, war) => wep.WeaponID == war.WeaponID).Where(w => w.Damage > 20).For(() => new { Race = "" }).GroupBy(w => w.Race).Select();
                Assert.AreEqual(sql, "select Race from Warrior join Weapon on (Weapon.WeaponID = Warrior.WeaponID) where (Weapon.Damage > 20) GROUP BY Race");

                context.From<Warrior>().Join<Weapon>((wep, war) => wep.WeaponID == war.WeaponID).Where(w => w.Damage > 20).For(() => new { Race = "", WeaponID = 0 }).GroupBy(w => w.Race).ThenBy(w => w.WeaponID).Select();
                Assert.AreEqual(sql, "select Race, WeaponID from Warrior join Weapon on (Weapon.WeaponID = Warrior.WeaponID) where (Weapon.Damage > 20) GROUP BY Race, WeaponID");
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
                Assert.AreEqual(sql, "select MAX(ID) AS ID from Warrior");

                // select the max id with grouping
                context.From<Warrior>().Max(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).Select();
                Assert.AreEqual(sql, "select MAX(ID) AS ID, Warrior.Race from Warrior GROUP BY Race");

                // select the max id with grouping
                context.From<Warrior>().Max(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).For<Warrior>().Select();
                Assert.AreEqual(sql, "select MAX(ID) AS ID, Warrior.Race from Warrior GROUP BY Race");

                context.From<Warrior>().Max(w => w.ID, "MaxID").For(() => new { MaxID = 0 }).Select();
                Assert.AreEqual(sql, "select MAX(ID) AS MaxID from Warrior");
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
                Assert.AreEqual(sql, "select MIN(ID) AS ID from Warrior");

                // select the min id with grouping
                context.From<Warrior>().Min(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).Select();
                Assert.AreEqual(sql, "select MIN(ID) AS ID, Warrior.Race from Warrior GROUP BY Race");

                // select the min id with grouping
                context.From<Warrior>().Min(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).For<Warrior>().Select();
                Assert.AreEqual(sql, "select MIN(ID) AS ID, Warrior.Race from Warrior GROUP BY Race");

                // select the min id
                context.From<Warrior>().Min(w => w.ID, "MinID").For(() => new { MinID = 0 }).Select();
                Assert.AreEqual(sql, "select MIN(ID) AS MinID from Warrior");
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
                Assert.AreEqual(sql, "select COUNT(ID) AS ID from Warrior");

                // select the min id with grouping
                context.From<Warrior>().Count(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).Select();
                Assert.AreEqual(sql, "select COUNT(ID) AS ID, Warrior.Race from Warrior GROUP BY Race");

                // select the min id with grouping
                context.From<Warrior>().Count(w => w.ID).Map(w => w.Race).GroupBy(w => w.Race).For<Warrior>().Select();
                Assert.AreEqual(sql, "select COUNT(ID) AS ID, Warrior.Race from Warrior GROUP BY Race");

                // select the min id
                context.From<Warrior>().Count(w => w.ID, "IdCount").For(() => new { IdCount = 0 }).Select();
                Assert.AreEqual(sql, "select COUNT(ID) AS IdCount from Warrior");
            }
        }

        [Test]
        public void SelectWithAliasMapping()
        {
            var expected = "select Orders.Freight as SpecialFreight, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

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
            var expected = "select Orders.Freight as SpecialFreight, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

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
            var expected = "select ProductID, Quantity from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

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
            var expected = "select Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

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
            var expected = "select Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

            var provider = new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected));
            using (var context = provider.Open())
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

            var provider = new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), expected));
            using (var context = provider.Open())
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
            var expected = "select ID, WeaponID, Race, SpecialSkill from Warrior where Warrior.Race In ('Elf','Dwarf')";

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
                string expected = "select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate asc";
                Assert.AreEqual(sql, expected);

                // join with generic order by
                context.From<Orders>().Join<Customers>((c, o) => c.CustomerID == o.CustomerID).Map(c => c.CustomerID).Map(c => c.EmployeeID).OrderBy<Orders>(o => o.OrderDate).Select();
                expected = "select Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate asc";
                Assert.AreEqual(sql, expected);

                // join with simple order by desc
                context.From<Orders>().OrderByDesc(o => o.OrderDate).Select();
                expected = "select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate desc";
                Assert.AreEqual(sql, expected);

                // join with generic order by desc
                context.From<Orders>().Join<Customers>((c, o) => c.CustomerID == o.CustomerID).Map(c => c.CustomerID).Map(c => c.EmployeeID).OrderByDesc<Orders>(o => o.OrderDate).Select();
                expected = "select Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate desc";
                Assert.AreEqual(sql, expected);

                // join with simple order by with simple then by
                context.From<Orders>().OrderBy(o => o.OrderDate).ThenBy(o => o.RequiredDate).Select();
                expected = "select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate asc , Orders.RequiredDate asc";
                Assert.AreEqual(sql, expected);

                // join with generic order by with simple then by
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderBy<Orders>(o => o.OrderDate)
                    .ThenBy(o => o.RequiredDate)
                    .Select();
                expected = "select Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate asc , Orders.RequiredDate asc";
                Assert.AreEqual(sql, expected);

                // join with simple order by desc with simple then by
                context.From<Orders>().OrderByDesc(o => o.OrderDate).ThenBy(o => o.RequiredDate).Select();
                expected = "select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate desc , Orders.RequiredDate asc";
                Assert.AreEqual(sql, expected);

                // join with generic order by desc with simple then by
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderByDesc<Orders>(o => o.OrderDate)
                    .ThenBy(o => o.RequiredDate)
                    .Select();
                expected = "select Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate desc , Orders.RequiredDate asc";
                Assert.AreEqual(sql, expected);

                // join with simple order by with generic then by
                context.From<Orders>().OrderBy(o => o.OrderDate).ThenBy<Orders>(o => o.RequiredDate).Select();
                expected = "select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate asc , Orders.RequiredDate asc";
                Assert.AreEqual(sql, expected);

                // join with generic order by with generic then by
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderBy<Orders>(o => o.OrderDate)
                    .ThenBy<Customers>(c => c.CompanyName)
                    .Select();
                expected = "select Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate asc , Customers.CompanyName asc";
                Assert.AreEqual(sql, expected);

                // join with simple order by desc with generic then by
                context.From<Orders>().OrderByDesc(o => o.OrderDate).ThenBy<Orders>(o => o.RequiredDate).Select();
                expected = "select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate desc , Orders.RequiredDate asc";
                Assert.AreEqual(sql, expected);

                // join with generic order by desc with generic then by
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderByDesc<Orders>(o => o.OrderDate)
                    .ThenBy<Customers>(c => c.CompanyName)
                    .Select();
                expected = "select Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate desc , Customers.CompanyName asc";
                Assert.AreEqual(sql, expected);

                // join with simple order by with generic then by desc
                context.From<Orders>().OrderBy(o => o.OrderDate).ThenByDesc<Orders>(o => o.RequiredDate).Select();
                expected = "select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate asc , Orders.RequiredDate desc";
                Assert.AreEqual(sql, expected);

                // join with generic order by with generic then by desc
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderBy<Orders>(o => o.OrderDate)
                    .ThenByDesc<Customers>(c => c.CompanyName)
                    .Select();
                expected = "select Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate asc , Customers.CompanyName desc";
                Assert.AreEqual(sql, expected);

                // join with simple order by desc with generic then by desc
                context.From<Orders>().OrderByDesc(o => o.OrderDate).ThenByDesc<Orders>(o => o.RequiredDate).Select();
                expected = "select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate desc , Orders.RequiredDate desc";
                Assert.AreEqual(sql, expected);

                // join with generic order by desc with generic then by desc
                context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderByDesc<Orders>(o => o.OrderDate)
                    .ThenByDesc<Customers>(c => c.CompanyName)
                    .Select();
                expected = "select Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate desc , Customers.CompanyName desc";
                Assert.AreEqual(sql, expected);
            }
        }
    }
}
