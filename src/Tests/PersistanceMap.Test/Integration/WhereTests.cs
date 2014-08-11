using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class WhereTests : TestBase
    {
        [Test]
        [Description("Select statement with a simple where operation")]
        public void Select_Where()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // where operation on previous type
                var query = context.From<Orders>()
                    .Map(o => o.OrderID)
                    .Join<OrderDetails>((d, o) => d.OrderID == o.OrderID)
                    .Where(o => o.Discount > 0);

                // execute the query
                var orders = query.Select<OrderDetails>();

                var sql = query.CompileQuery<OrderDetails>().Flatten();
                var expected = "select Orders.OrderID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID) where (OrderDetails.Discount > '0')";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("select statement with where operation for generic type")]
        public void Select_WhereForGenericType()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // where operation on new generic type
                var query = context.From<Orders>()
                    .Map(o => o.OrderID)
                    .Join<OrderDetails>((d, o) => d.OrderID == o.OrderID)
                    .Where<Orders>(o => o.Freight > 0);

                // execute the query
                var orders = query.Select<OrderDetails>();

                var sql = query.CompileQuery<OrderDetails>().Flatten();
                var expected = "select Orders.OrderID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID) where (Orders.Freight > '0')";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("select statement with where operation for multiple generic types")]
        public void Select_WhereForMultipeGnenericTypes()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // where operation on new generic type
                var query = context.From<Employees>()
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Where<Employees, Customers>((e, c) => e.EmployeeID != c.EmployeeID);

                // execute the query
                var employees = query.Select<Employees>();

                var sql = query.CompileQuery<Employees>().Flatten();
                var expected = "select Employees.EmployeeID, Employees.Address, Employees.City, Employees.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Employees join Orders on (Orders.EmployeeID = Employees.EmployeeID) join Customers on (Customers.CustomerID = Orders.CustomerID) where (Employees.EmployeeID <> Customers.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(employees.Any());
            }
        }

        [Test]
        [Description("select statement with a where and a simple and operation")]
        public void Select_Where_WithSimpleAnd()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // join using on and or
                var query = context.From<Orders>()
                    .Where(p => p.CustomerID.StartsWith("se"))
                    .And(o => o.ShipCity == "London");

                // execute the query
                var orders = query.Select<Orders>();

                var sql = query.CompileQuery<Orders>().Flatten();
                var expected = "select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders where Orders.CustomerID like 'se%' and (Orders.ShipCity = 'London')";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("select statement with where operation and a simple generic and operation")]
        public void Select_Where_WithGenericAnd()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // where operation with and
                var query = context.From<Orders>()
                    .Map(o => o.OrderID)
                    .Join<OrderDetails>((d, o) => d.OrderID == o.OrderID)
                    .Where<Orders, OrderDetails>((o, od) => o.OrderID == od.OrderID)
                    .And<OrderDetails>((od) => od.Quantity > 10);

                // execute the query
                var orderdetails = query.Select();

                var sql = query.CompileQuery<OrderDetails>().Flatten();
                var expected = "select Orders.OrderID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID) where (Orders.OrderID = OrderDetails.OrderID) and (OrderDetails.Quantity > 10)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orderdetails.Any());
            }
        }

        [Test]
        [Description("select statement with a where operationion and a and operation mapped to 2 differen types")]
        public void Select_Where_WithMultyGenericAnd()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // where operation with and
                var query = context.From<Employees>()
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                    .Map(o => o.EmployeeID)
                    .Map(o => o.CustomerID)
                    .Join<Customers>((c, o) => o.CustomerID == o.CustomerID)
                    .Where(c => c.ContactName.Contains("a"))
                    .And<Customers, Employees>((c, e) => c.EmployeeID == e.EmployeeID);

                // execute the query
                var orders = query.Select<Orders>();

                var sql = query.CompileQuery<Orders>().Flatten();
                var expected = "select Orders.EmployeeID, Orders.CustomerID, OrderID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Employees join Orders on (Orders.EmployeeID = Employees.EmployeeID) join Customers on (Orders.CustomerID = Orders.CustomerID) where Customers.ContactName like '%a%' and (Customers.EmployeeID = Employees.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("select statement with a where and a simple or operation")]
        public void Select_Where_WithSimpleOr()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // join using on and or
                var query = context.From<Orders>()
                    .Where(p => p.CustomerID.StartsWith("P"))
                    .Or(o => o.ShipCity == "London");

                // execute the query
                var orders = query.Select<Orders>();

                var sql = query.CompileQuery<Orders>().Flatten();
                var expected = "select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders where Orders.CustomerID like 'P%' or (Orders.ShipCity = 'London')";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("select statement with a where and a simple or operation")]
        public void Select_Where_WithSimpleGenericOr()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // join using on and or
                var query = context.From<Orders>()
                    .Where(p => p.CustomerID.StartsWith("P"))
                    .Or<Orders>(o => o.ShipCity == "Paris");

                // execute the query
                var orders = query.Select<Orders>();

                var sql = query.CompileQuery<Orders>().Flatten();
                var expected = "select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders where Orders.CustomerID like 'P%' or (Orders.ShipCity = 'Paris')";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("select statement with a where and a simple or operation")]
        public void Select_Where_WithSimpleGenericOr2()
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
                    .Map(o => o.Region)
                    .Map(o => o.CustomerID)
                    .Map(o => o.Country)
                    .Where<Employees, Customers>((e, o) => e.EmployeeID == o.EmployeeID)
                    .Or<Orders>(o => o.Freight > 0);

                // execute the query
                var orders = query.Select<Customers>();

                var sql = query.CompileQuery<Customers>().Flatten();
                var expected = "select Employees.EmployeeID, Employees.Address, Employees.City, Employees.PostalCode, Customers.Region, Customers.CustomerID, Customers.Country, CompanyName, ContactName, ContactTitle, Phone, Fax from Employees join Orders on (Orders.EmployeeID = Employees.EmployeeID) join Customers on (Customers.CustomerID = Orders.CustomerID) where (Employees.EmployeeID = Customers.EmployeeID) or (Orders.Freight > '0')";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("select statement with a where and a or operation containing two entities")]
        public void Select_Where_WithSimpleComplexGenericOr()
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
                    .Map(o => o.Region)
                    .Map(o => o.CustomerID)
                    .Map(o => o.Country)
                    .Where<Employees, Customers>((e, o) => e.EmployeeID == o.EmployeeID)
                    .Or<Employees>((c, e) => c.EmployeeID == e.EmployeeID);

                // execute the query
                var orders = query.Select<Customers>();

                var sql = query.CompileQuery<Customers>().Flatten();
                var expected = "select Employees.EmployeeID, Employees.Address, Employees.City, Employees.PostalCode, Customers.Region, Customers.CustomerID, Customers.Country, CompanyName, ContactName, ContactTitle, Phone, Fax from Employees join Orders on (Orders.EmployeeID = Employees.EmployeeID) join Customers on (Customers.CustomerID = Orders.CustomerID) where (Employees.EmployeeID = Customers.EmployeeID) or (Customers.EmployeeID = Employees.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("select statement with a where operation and a or operation that has two genereic parameters")]
        public void Select_Where_WithComplexGenericOr()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // join using on and and
                //TODO: And allways returns false! create connection that realy works!
                var query = context.From<Customers>()
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>((o, c) => o.EmployeeID == c.EmployeeID)
                    .Join<Employees>((e, o) => e.EmployeeID == o.EmployeeID)
                    .Where(e => e.FirstName.Contains("Davolio"))
                    .Or<Customers, Employees>((c, e) => c.EmployeeID == e.EmployeeID);

                // execute the query
                var orders = query.Select<Employees>();

                var sql = query.CompileQuery<Employees>().Flatten();
                var expected = "select Customers.EmployeeID, Customers.Address, Customers.City, Customers.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Customers join Orders on (Orders.EmployeeID = Customers.EmployeeID) join Employees on (Employees.EmployeeID = Orders.EmployeeID) where Employees.FirstName like '%Davolio%' or (Customers.EmployeeID = Employees.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("select statement with a where operationion and a and operation mapped to 2 differen types and containing an alias")]
        public void Select_Where_WithMultyGenericAnd_WithAlias()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // where operation with and
                var query = context.From<Employees>()
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                    .Map(o => o.EmployeeID)
                    .Map(o => o.CustomerID)
                    .Join<Customers>((c, o) => o.CustomerID == o.CustomerID, alias: "cust")
                    .Where(c => c.ContactName.Contains("a"))
                    .And<Customers, Employees>((c, e) => c.EmployeeID == e.EmployeeID, alias: "cust");

                // execute the query
                var orders = query.Select<Orders>();

                var sql = query.CompileQuery<Orders>().Flatten();
                var expected = "select Orders.EmployeeID, Orders.CustomerID, OrderID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Employees join Orders on (Orders.EmployeeID = Employees.EmployeeID) join Customers cust on (Orders.CustomerID = Orders.CustomerID) where cust.ContactName like '%a%' and (cust.EmployeeID = Employees.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("select statement with a where operationion and a and operation mapped to 2 differen types and containing an alias for the source")]
        public void Select_Where_WithMultyGenericAnd_WithAliasForSource()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // where operation with and
                var query = context.From<Employees>("emp")
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID, source: "emp")
                    .Map(o => o.EmployeeID)
                    .Map(o => o.CustomerID)
                    .Join<Customers>((c, o) => o.CustomerID == o.CustomerID)
                    .Where(c => c.ContactName.Contains("a"))
                    .And<Customers, Employees>((c, e) => c.EmployeeID == e.EmployeeID, source: "emp");

                // execute the query
                var orders = query.Select<Orders>();

                var sql = query.CompileQuery<Orders>().Flatten();
                var expected = "select Orders.EmployeeID, Orders.CustomerID, OrderID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Employees emp join Orders on (Orders.EmployeeID = emp.EmployeeID) join Customers on (Orders.CustomerID = Orders.CustomerID) where Customers.ContactName like '%a%' and (Customers.EmployeeID = emp.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }
    }
}
