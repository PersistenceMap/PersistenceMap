using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;
using System.Linq;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class JoinTests : TestBase
    {
        [Test]
        public void Select_Join()
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
        public void Select_Join_InFrom()
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
        public void Select_Join_WithOn_WithProjection()
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
        public void Select_Join_WithAlias_WithMaps_WithProjection()
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
        public void Select_Join_WithAliasInFrom_WithMaps_WithProjection()
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
        public void Select_Join_WithOnAndInJoin_WithProjection()
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
        public void Select_Join_WithAnd_WithProjection()
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
        public void Select_Join_WithAnd_WithAlias_WithProjection()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                //TODO: And allways returns false! create connection that realy works!
                var query = context.From<Customers>("customer")
                    .Join<Orders>("ord", "customer", (o, c) => o.EmployeeID == c.EmployeeID)
                    .Join<Employees>("empl", "ord", (e, o) => e.EmployeeID == o.EmployeeID)
                    .And<Customers>("empl", "customer", (c, e) => c.EmployeeID == e.EmployeeID)
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode);

                var sql = query.CompileQuery<Employees>().Flatten();
                var expected = "select empl.EmployeeID, empl.Address, empl.City, empl.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Customers customer join Orders ord on (ord.EmployeeID = customer.EmployeeID) join Employees empl on (empl.EmployeeID = ord.EmployeeID) and (empl.EmployeeID = customer.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                // execute the query
                var employees = query.Select<Employees>();

                Assert.IsTrue(employees.Any());
            }
        }

        [Test]
        public void Select_Join_WithOn_WithMap()
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
        public void Select_Joins_WithOn_WithMaps()
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

        [Test]
        [Description("A select statemenent with a join and a or operation")]
        public void Select_Join_WithGenericOr()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // join using on and or
                var query = context.From<Employees>()
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Or<Employees>((c, e) => c.EmployeeID == e.EmployeeID)
                    .Map(o => o.Region)
                    .Map(o => o.CustomerID)
                    .Map(o => o.Country);

                // execute the query
                var customers = query.Select<Customers>();

                var sql = query.CompileQuery<Customers>().Flatten();
                var expected = "select Employees.EmployeeID, Employees.Address, Employees.City, Employees.PostalCode, Customers.Region, Customers.CustomerID, Customers.Country, CompanyName, ContactName, ContactTitle, Phone, Fax from Employees join Orders on (Orders.EmployeeID = Employees.EmployeeID) join Customers on (Customers.CustomerID = Orders.CustomerID) or (Customers.EmployeeID = Employees.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(customers.Any());
            }
        }
        
        [Test]
        [Description("select with a join that connects to a other table than the previous")]
        public void Select_Join_ToOtherTable()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // select with a join that connects to a other table than the previous
                var query = context.From<Employees>()
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                    .Join<Customers, Employees>((c, e) => c.EmployeeID == e.EmployeeID);

                // execute the query
                var customers = query.Select<Employees>();

                var sql = query.CompileQuery<Employees>().Flatten();
                var expected = "select Employees.EmployeeID, Employees.Address, Employees.City, Employees.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Employees join Orders on (Orders.EmployeeID = Employees.EmployeeID) join Customers on (Customers.EmployeeID = Employees.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(customers.Any());
            }
        }

        [Test]
        [Description("select with a join that contains a alias connects to a other table than the previous")]
        public void Select_Join_WithAlias_ToOtherTable()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // select with a join that contains a alias connects to a other table than the previous
                var query = context.From<Employees>()
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                    .Join<Customers, Employees>("cust", (c, e) => c.EmployeeID == e.EmployeeID);

                // execute the query
                var customers = query.Select<Employees>();

                var sql = query.CompileQuery<Employees>().Flatten();
                var expected = "select Employees.EmployeeID, Employees.Address, Employees.City, Employees.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Employees join Orders on (Orders.EmployeeID = Employees.EmployeeID) join Customers cust on (cust.EmployeeID = Employees.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(customers.Any());
            }
        }

        [Test]
        [Description("select with a join that containes an alias and connects to a other table than the previous that also contains an alias")]
        public void Select_Join_WithAlias_ToOtherTable_WithAlias()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // select with a join that containes an alias and connects to a other table than the previous that also contains an alias
                var query = context.From<Employees>("emp")
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>(null, "emp", (o, e) => o.EmployeeID == e.EmployeeID)
                    .Join<Customers, Employees>("cust", "emp", (c, e) => c.EmployeeID == e.EmployeeID);

                // execute the query
                var customers = query.Select<Employees>();

                var sql = query.CompileQuery<Employees>().Flatten();
                var expected = "select emp.EmployeeID, emp.Address, emp.City, emp.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Employees emp join Orders on (Orders.EmployeeID = emp.EmployeeID) join Customers cust on (cust.EmployeeID = emp.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(customers.Any());
            }
        }
    }
}
