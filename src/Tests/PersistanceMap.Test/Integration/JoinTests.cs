using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;
using System.Linq;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class JoinTests : TestBase
    {
        [Test]
        public void SimpleJoinWithProjectionTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var orders = context.From<Orders>()
                    .Join<OrderDetails>((det, order) => det.OrderID == order.OrderID)
                    .Select<OrderWithDetail>();

                Assert.IsTrue(orders.Any());
                Assert.IsFalse(string.IsNullOrEmpty(orders.First().ShipName));
                Assert.IsTrue(orders.First().ProductID > 0);
            }
        }

        [Test]
        public void SimpleJoinInFromWithProjectionTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var query = context.From<Orders, OrderDetails>((det, order) => det.OrderID == order.OrderID);

                var sql = "select CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderWithDetail>().Flatten(), sql);

                // execute the query
                var orders = query.Select<OrderWithDetail>();

                Assert.IsTrue(orders.Any());
                Assert.IsFalse(string.IsNullOrEmpty(orders.First().ShipName));
                Assert.IsTrue(orders.First().ProductID > 0);
            }
        }

        [Test]
        public void SimpleJoinWithOnWithProjectionTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var orders = context.From<Orders>()
                    .Join<OrderDetails>((det, order) => det.OrderID == order.OrderID)
                    .Select<OrderWithDetail>();

                Assert.IsTrue(orders.Any());
                Assert.IsFalse(string.IsNullOrEmpty(orders.First().ShipName));
                Assert.IsTrue(orders.First().ProductID > 0);
            }
        }

        [Test]
        public void SimpleJoinWithOnAndIdentifierOptionWithProjectionTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var query = context.From<Orders>()
                    .Join<OrderDetails>("detail", (det, order) => det.OrderID == order.OrderID);

                // execute the query
                var orders = query.Select<OrderWithDetail>();

                var sql = query.CompileQuery<OrderWithDetail>().Flatten();
                var expected = "select CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails detail on (detail.OrderID = Orders.OrderID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
                Assert.IsFalse(string.IsNullOrEmpty(orders.First().ShipName));
                Assert.IsTrue(orders.First().ProductID > 0);
            }
        }

        [Test]
        public void SimpleJoinWithOnAndIdentifiersWithProjectionTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var query = context.From<Orders>("orders")
                    .Map(o => o.OrderID)
                    .Join<OrderDetails>("detail", "orders", (detail, order) => detail.OrderID == order.OrderID);

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderWithDetail>().Flatten(), "select orders.OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders orders join OrderDetails detail on (detail.OrderID = orders.OrderID)");

                // execute the query
                var orders = query.Select<OrderWithDetail>();

                Assert.IsTrue(orders.Any());
                Assert.IsFalse(string.IsNullOrEmpty(orders.First().ShipName));
                Assert.IsTrue(orders.First().ProductID > 0);
            }
        }

        [Test]
        public void SimpleJoin_WithOr_WithProjection()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                //TODO: Or allways returns false! create connection that realy works!
                var orders = context.From<Orders>()
                    .Join<OrderDetails>((detail, order) => detail.OrderID == order.OrderID)
                    .Or<Orders>((detail, order) => false)
                    .Select<OrderWithDetail>();

                //TODO: Or allways returns false! create connection that realy works!
                Assert.Fail();

                Assert.IsTrue(orders.Any());
                Assert.IsFalse(string.IsNullOrEmpty(orders.First().ShipName));
                Assert.IsTrue(orders.First().ProductID > 0);
            }
        }

        [Test]
        public void SimpleJoin_WithOnAndInJoin_WithProjection()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var query = context.From<Orders>()
                    .Join<OrderDetails>((detail, order) => detail.OrderID == order.OrderID && detail.Quantity > 5);

                var sql = "select CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on ((OrderDetails.OrderID = Orders.OrderID) AND (OrderDetails.Quantity > 5))";
                var output = query.CompileQuery<OrderWithDetail>().Flatten();

                // check the compiled sql
                Assert.AreEqual(output, sql);

                // execute the query
                var orders = query.Select<OrderWithDetail>();

                Assert.IsTrue(orders.Any());
                Assert.IsFalse(string.IsNullOrEmpty(orders.First().ShipName));
                Assert.IsTrue(orders.First().ProductID > 0);
            }
        }

        [Test]
        public void SimpleJoin_WithAnd_WithProjection()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                // get all customers that are employees and have ordered
                var query = context.From<Customers>()
                    .Join<Orders>((o, c) => o.EmployeeID == c.EmployeeID)
                    .Join<Employees>((e, o) => e.EmployeeID == o.EmployeeID)
                    .And<Customers>((e, c) => e.EmployeeID == c.EmployeeID)
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode);
                
                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<Employees>().Flatten(), "select Employees.EmployeeID, Employees.Address, Employees.City, Employees.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Customers join Orders on (Orders.EmployeeID = Customers.EmployeeID) join Employees on (Employees.EmployeeID = Orders.EmployeeID) and (Employees.EmployeeID = Customers.EmployeeID)");

                // execute the query
                var emloyees = query.Select<Employees>();
                
                Assert.IsTrue(emloyees.Any());
            }
        }

        [Test]
        public void SimpleJoin_WithAnd_WithAlias_WithProjection()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                //TODO: And allways returns false! create connection that realy works!
                var query = context.From<EmployeeTerritories>("et1")
                    .Join<Employees>("e1", "et1", (e1, et1) => e1.EmployeeID == et1.EmployeeID)
                    //TODO: because of AliasMap in Join a type can't join to the same type
                    //TODO: IJoinQueryProvider<Employees>.Join<Employees>()....
                    .Join<Employees>("e2", "e1", (e2, e1) => e2.ReportsTo == e1.EmployeeID)
                    .Join<EmployeeTerritories>("et2", "e2", (et2, e2) => et2.EmployeeID == e2.EmployeeID)
                    .And<EmployeeTerritories>("et2", "et1", (et2, et1) => et2.TerritoryID == et1.TerritoryID);

                var sql = "";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<Employees>().Flatten(), sql);

                // execute the query
                var employees = query.Select<Employees>();

                Assert.IsTrue(employees.Any());
            }
        }

        [Test]
        public void SimpleJoin_WithOn_WithMapTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                // join using include
                var orders = context
                    .From<Orders>()
                    .Join<OrderDetails>((det, order) => det.OrderID == order.OrderID)
                    .Map(i => i.OrderID)
                    .Select<OrderDetails>();

                /* *Expected Query*
                 select OrderDetails.OrderID, ProductID, UnitPrice, Quantity, Discount 
                 from Orders 
                 join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)
                */

                Assert.IsTrue(orders.Any());
                Assert.IsTrue(orders.First().OrderID > 0);
                Assert.IsTrue(orders.First().ProductID > 0);
            }
        }

        [Test]
        public void MultipleJoinsWithOnAndIncludeTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                // multiple joins using On<T> with include
                var orders = context
                    .From<Orders>()
                    .Join<OrderDetails>((det, order) => det.OrderID == order.OrderID)
                    .Join<Products>((product, det) => product.ProductID == det.ProductID)
                    .Map(p => p.ProductID)
                    .Map(p => p.UnitPrice)
                    .Select<OrderWithDetail>();

                /* *Expected Query*
                 select Products.ProductID, Products.UnitPrice, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, Quantity, Discount 
                 from Orders 
                 join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)
                 join Products on (Products.ProductID = OrderDetails.ProductID)
                */

                Assert.IsTrue(orders.Any());
                Assert.IsFalse(string.IsNullOrEmpty(orders.First().ShipName));
                Assert.IsTrue(orders.First().ProductID > 0);
            }
        }
    }
}
