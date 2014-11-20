﻿using NUnit.Framework;
using PersistanceMap.Test.TableTypes;
using System;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class SelectTests : TestBase
    {

        [Test]
        public void SimpleSelectTest()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                var orders = context.Select<Orders>();
                /* *Expected Query*
                select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry 
                from Orders
                */

                Assert.IsNotNull(orders);
                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        public void SimpleNongenericSelectWithJoinTest()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                var query = context.From<Orders>().Map(o => o.OrdersID).Join<OrderDetails>((d, o) => d.OrdersID == o.OrdersID);

                var sql = "select Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery().Flatten(), sql);

                // execute the query
                var orders = query.Select();

                Assert.IsNotNull(orders);
                Assert.IsTrue(orders.Any());
            }
        }


        [Test]
        public void SelectWithMax()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // select the max id
                var query1 = context.From<Orders>().Max(w => w.OrdersID);

                var expected = "select MAX(OrdersID) AS OrdersID from Orders";
                var sql = query1.CompileQuery().Flatten();
                Assert.AreEqual(sql, expected);

                var orders1 = query1.Select();

                Assert.IsNotNull(orders1);
                Assert.IsTrue(orders1.Any());
                Assert.IsTrue(orders1.First().OrdersID > 0);



                // select the max id with grouping
                var query2 = context.From<Orders>().Max(w => w.OrdersID).Map(w => w.CustomerID).GroupBy(w => w.CustomerID).For(() => new { OrdersID = 0, CustomerID = "" });

                expected = "select MAX(OrdersID) AS OrdersID, Orders.CustomerID from Orders GROUP BY CustomerID";
                sql = query2.CompileQuery().Flatten();
                Assert.AreEqual(sql, expected);

                var orders2 = query2.Select();

                Assert.IsNotNull(orders2);
                Assert.IsTrue(orders2.Any());
                Assert.IsTrue(orders2.First().OrdersID > 0);



                // select the max id with grouping
                var query3 = context.From<Orders>().Max(w => w.OrdersID, "MaxID").For(() => new { MaxID = 0 });

                expected = "select MAX(OrdersID) AS MaxID from Orders";
                sql = query3.CompileQuery().Flatten();
                Assert.AreEqual(sql, expected);

                var orders3 = query3.Select();

                Assert.IsNotNull(orders3);
                Assert.IsTrue(orders3.Any());
                Assert.IsTrue(orders3.First().MaxID > 0);
            }
        }

        [Test]
        public void SelectWithMin()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // select the min id
                var query1 = context.From<Orders>().Min(w => w.OrdersID);
                var sql = "select MIN(OrdersID) AS OrdersID from Orders";

                Assert.AreEqual(query1.CompileQuery().Flatten(), sql);

                var orders1 = query1.Select();

                Assert.IsNotNull(orders1);
                Assert.IsTrue(orders1.Any());
                Assert.IsTrue(orders1.First().OrdersID > 0);



                // select the min id with grouping
                var query2 = context.From<Orders>().Min(w => w.OrdersID).Map(w => w.CustomerID).GroupBy(w => w.CustomerID).For(() => new { OrdersID = 0, CustomerID = "" });
                sql = "select MIN(OrdersID) AS OrdersID, Orders.CustomerID from Orders GROUP BY CustomerID";

                Assert.AreEqual(query2.CompileQuery().Flatten(), sql);

                var orders2 = query2.Select();

                Assert.IsNotNull(orders2);
                Assert.IsTrue(orders2.Any());
                Assert.IsTrue(orders2.First().OrdersID > 0);



                // select the min id with grouping
                var query3 = context.From<Orders>().Min(w => w.OrdersID, "MinID").For(() => new { MinID = 0 });
                sql = "select MIN(OrdersID) AS MinID from Orders";

                Assert.AreEqual(query3.CompileQuery().Flatten(), sql);

                var orders3 = query3.Select();

                Assert.IsNotNull(orders3);
                Assert.IsTrue(orders3.Any());
                Assert.IsTrue(orders3.First().MinID > 0);
            }
        }

        [Test]
        public void SelectWithCount()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // select the min id
                var query1 = context.From<Orders>().Count(w => w.OrdersID);
                var sql = "select COUNT(OrdersID) AS OrdersID from Orders";

                Assert.AreEqual(query1.CompileQuery().Flatten(), sql);

                var orders1 = query1.Select();

                Assert.IsNotNull(orders1);
                Assert.IsTrue(orders1.Any());
                Assert.IsTrue(orders1.First().OrdersID > 0);



                // select the min id with grouping
                var query2 = context.From<Orders>().Count(w => w.OrdersID).Map(w => w.CustomerID).GroupBy(w => w.CustomerID).For(() => new { OrdersID = 0, CustomerID = "" });
                sql = "select COUNT(OrdersID) AS OrdersID, Orders.CustomerID from Orders GROUP BY CustomerID";

                Assert.AreEqual(query2.CompileQuery().Flatten(), sql);

                var orders2 = query2.Select();

                Assert.IsNotNull(orders2);
                Assert.IsTrue(orders2.Any());
                Assert.IsTrue(orders2.First().OrdersID > 0);



                // select the min id with grouping
                var query3 = context.From<Orders>().Count(w => w.OrdersID, "IdCount").For(() => new { IdCount = 0 });
                sql = "select COUNT(OrdersID) AS IdCount from Orders";

                Assert.AreEqual(query3.CompileQuery().Flatten(), sql);

                var orders3 = query3.Select();

                Assert.IsNotNull(orders3);
                Assert.IsTrue(orders3.Any());
                Assert.IsTrue(orders3.First().IdCount > 0);
            }
        }


        [Test]
        public void SelectWithFormatException()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // CustomerID should be a string
                var query = context.From<Orders>().Max(w => w.OrdersID).Map(w => w.CustomerID).GroupBy(w => w.CustomerID).For(() => new { OrdersID = 0, CustomerID = 0 });

                Assert.Throws<FormatException>(() => query.Select());
            }
        }


        [Test]
        [Description("select statement that compiles from a FOR operation with a anonym object defining the resultset entries and mapped to a defined type")]
        public void SelectForAnonymToDefinedType()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // select only the properties that are defined in the anony object
                var query = context.From<Orders>()
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)
                    .For(() => new
                    {
                        ProductID = 0,
                        Quantity = 0
                    });

                var anonymous = query.Select<OrderDetails>();

                var sql = query.CompileQuery().Flatten();
                var expected = "select ProductID, Quantity from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

                Assert.AreEqual(sql, expected);
                Assert.IsTrue(anonymous.Any());
                Assert.IsTrue(anonymous.First() is OrderDetails);
                Assert.IsTrue(anonymous.First().ProductID > 0);
                Assert.IsTrue(anonymous.First().Quantity > 0);
            }
        }

        [Test]
        [Description("select statement that compiles from a FOR operation with a anonym object defining the resultset entries and mapped to a defined type using a delegate")]
        public void SelectForAnonymCastToDefinedTypeDelegate()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // select only the properties that are defined in the anony object
                var query = context.From<Orders>()
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)
                    .For(() => new
                    {
                        ProductID = 0,
                        Quantity = 0
                    });

                var anonymous = query.Select(tmp => new OrderDetails { ProductID = tmp.ProductID, Quantity = tmp.Quantity });

                var sql = query.CompileQuery().Flatten();
                var expected = "select ProductID, Quantity from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

                Assert.AreEqual(sql, expected);
                Assert.IsTrue(anonymous.Any());
                Assert.IsTrue(anonymous.First() is OrderDetails);
                Assert.IsTrue(anonymous.First().ProductID > 0);
                Assert.IsTrue(anonymous.First().Quantity > 0);
            }
        }

        [Test]
        public void SelectWithoutEmptyConstructor()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                var orders = context.Select<PersistanceMap.Test.TableTypes.InvalidTypes.Orders>();

                Assert.IsNotNull(orders);
                Assert.IsTrue(orders.Any());

                var orders2 = context.Select<Orders>();

                Assert.IsNotNull(orders2);
                Assert.IsTrue(orders2.Any());
            }
        }




        [Test]
        public void SelectFromDirect()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                var query = context.From<Products>();

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<Products>().Flatten(), "select ProductID, ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued from Products");

                // execute the query
                var prsAbt = query.Select<Products>();

                Assert.IsTrue(prsAbt.Any());
            }
        }

        [Test]
        public void SelectFromWithAliasInFrom()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                var query = context.From<Products>("prod");

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<Products>().Flatten(), "select ProductID, ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued from Products prod");

                // execute the query
                var products = query.Select<Products>();

                Assert.IsTrue(products.Any());
            }
        }

        [Test]
        public void SelectFromWithJoinWithAlias()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                // join using identifiers in the on expression
                var query = context.From<Orders>("orders")
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((det, order) => det.OrdersID == order.OrdersID, "detail", "orders");

                var sql = "select orders.OrdersID, ProductID, UnitPrice, Quantity, Discount from Orders orders join OrderDetails detail on (detail.OrdersID = orders.OrdersID)";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderDetails>().Flatten(), sql);

                // execute the query
                var orders = query.Select<OrderDetails>();

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        public void SelectFromWithAlias()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                // multiple joins using On<T> with include and includeoperation in from expression
                var query = context
                    .From<Orders>()
                    .Map(p => p.OrdersID)
                    .Join<OrderDetails>((det, order) => det.OrdersID == order.OrdersID)
                    .Join<Products>((product, det) => product.ProductID == det.ProductID)
                    .Map(p => p.ProductID)
                    .Map(p => p.UnitPrice);

                var sql = query.CompileQuery<OrderDetails>().Flatten();
                var expected = "select Orders.OrdersID, Products.ProductID, Products.UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID) join Products on (Products.ProductID = OrderDetails.ProductID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                // execute the query
                var orders = query.Select<OrderDetails>();

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("A select statement where the map takes the alias from the previous operation")]
        public void FromWithIncludeWithAliasMapWithoutAlias()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                var query = context
                    .From<Orders>("ord")
                    .Map(p => p.OrdersID)
                    //TODO: Join has to check the previous for the alias? "ord"
                    .Join<OrderDetails>((det, order) => det.OrdersID == order.OrdersID, source: "ord")
                    .Join<Products>((product, det) => product.ProductID == det.ProductID)
                    .Map(p => p.ProductID)
                    .Map(p => p.UnitPrice);

                var sql = query.CompileQuery<OrderDetails>().Flatten();
                var expected = "select ord.OrdersID, Products.ProductID, Products.UnitPrice, Quantity, Discount from Orders ord join OrderDetails on (OrderDetails.OrdersID = ord.OrdersID) join Products on (Products.ProductID = OrderDetails.ProductID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                // execute the query
                var orders = query.Select<OrderDetails>();

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        public void IncludeWithWrongLambdaExpressionFailTest()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // fail test because Include doesn't return a property witch ends in a wrong sql statement
                var query = context.From<Orders>()
                    .Join<OrderDetails>((detail, order) => detail.OrdersID == order.OrdersID)
                    // this has to fail!
                    .Map(i => i.OrdersID != 1);

                Assert.Throws<SqlException>(() => query.Select<OrderWithDetailExtended>());
            }
        }

        [Test]
        public void JoinWithIndexerInMember()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {

                // product with indexer (this[string])
                var products = context.From<Products>().Select<ProductsWithIndexer>();

                Assert.IsTrue(products.Any());
            }
        }












        [Test]
        public void SelectJoin()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                var orders = context.From<Orders>()
                    .Join<OrderDetails>((det, order) => det.OrdersID == order.OrdersID)
                    .Select<OrderWithDetail>();

                Assert.IsTrue(orders.Any());
                Assert.IsFalse(string.IsNullOrEmpty(orders.First().ShipName));
                Assert.IsTrue(orders.First().ProductID > 0);
            }
        }

        [Test]
        public void SelectJoinInFrom()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                var query = context.From<Orders, OrderDetails>((det, order) => det.OrdersID == order.OrdersID);

                var sql = "select CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

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
        public void SelectJoinWithOnWithProjection()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                var orders = context.From<Orders>()
                    .Join<OrderDetails>((det, order) => det.OrdersID == order.OrdersID)
                    .Select<OrderWithDetail>();

                Assert.IsTrue(orders.Any());
                Assert.IsFalse(string.IsNullOrEmpty(orders.First().ShipName));
                Assert.IsTrue(orders.First().ProductID > 0);
            }
        }

        [Test]
        public void SelectJoinWithAliasWithMapsWithProjection()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                var query = context.From<Orders>()
                    .Join<OrderDetails>((det, order) => det.OrdersID == order.OrdersID, "detail");

                // execute the query
                var orders = query.Select<OrderWithDetail>();

                var sql = query.CompileQuery<OrderWithDetail>().Flatten();
                var expected = "select CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails detail on (detail.OrdersID = Orders.OrdersID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
                Assert.IsFalse(string.IsNullOrEmpty(orders.First().ShipName));
                Assert.IsTrue(orders.First().ProductID > 0);
            }
        }

        [Test]
        public void SelectJoinWithAliasInFromWithMapsWithProjection()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                var query = context.From<Orders>("orders")
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((detail, order) => detail.OrdersID == order.OrdersID, "detail", "orders");

                var sql = query.CompileQuery<OrderWithDetail>().Flatten();
                var expected = "select CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders orders join OrderDetails detail on (detail.OrdersID = orders.OrdersID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                // execute the query
                var orders = query.Select<OrderWithDetail>();

                Assert.IsTrue(orders.Any());
                Assert.IsFalse(string.IsNullOrEmpty(orders.First().ShipName));
                Assert.IsTrue(orders.First().ProductID > 0);
            }
        }

        [Test]
        public void SelectJoinWithOnAndInJoinWithProjection()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                var query = context.From<Orders>()
                    .Join<OrderDetails>((detail, order) => detail.OrdersID == order.OrdersID && detail.Quantity > 5);

                var sql = "select CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on ((OrderDetails.OrdersID = Orders.OrdersID) AND (OrderDetails.Quantity > 5))";
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
        public void SelectJoinWithAndWithProjection()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                // get all customers that are Employee and have ordered
                var query = context.From<Customers>()
                    .Join<Orders>((o, c) => o.EmployeeID == c.EmployeeID)
                    .Join<Employee>((e, o) => e.EmployeeID == o.EmployeeID)
                    .And<Customers>((e, c) => e.EmployeeID == c.EmployeeID)
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode);

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<Employee>().Flatten(), "select Employee.EmployeeID, Employee.Address, Employee.City, Employee.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Customers join Orders on (Orders.EmployeeID = Customers.EmployeeID) join Employee on (Employee.EmployeeID = Orders.EmployeeID) and (Employee.EmployeeID = Customers.EmployeeID)");

                // execute the query
                var emloyees = query.Select<Employee>();

                Assert.IsTrue(emloyees.Any());
            }
        }

        [Test]
        public void SelectJoinWithAndWithAliasWithProjection()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                //TODO: And allways returns false! create provider that realy works!
                var query = context.From<Customers>("customer")
                    .Join<Orders>((o, c) => o.EmployeeID == c.EmployeeID, "ord", "customer")
                    .Join<Employee>((e, o) => e.EmployeeID == o.EmployeeID, "empl", "ord")
                    .And<Customers>((e, c) => e.EmployeeID == c.EmployeeID, "empl", "customer")
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode);

                var sql = query.CompileQuery<Employee>().Flatten();
                var expected = "select empl.EmployeeID, empl.Address, empl.City, empl.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Customers customer join Orders ord on (ord.EmployeeID = customer.EmployeeID) join Employee empl on (empl.EmployeeID = ord.EmployeeID) and (empl.EmployeeID = customer.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                // execute the query
                var employees = query.Select<Employee>();

                Assert.IsTrue(employees.Any());
            }
        }

        [Test]
        public void SelectJoinWithOnWithMap()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                // join using include
                var orders = context
                    .From<Orders>()
                    .Join<OrderDetails>((det, order) => det.OrdersID == order.OrdersID)
                    .Map(i => i.OrdersID)
                    .Select<OrderDetails>();

                /* *Expected Query*
                 select OrderDetails.OrdersID, ProductID, UnitPrice, Quantity, Discount 
                 from Orders 
                 join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)
                */

                Assert.IsTrue(orders.Any());
                Assert.IsTrue(orders.First().OrdersID > 0);
                Assert.IsTrue(orders.First().ProductID > 0);
            }
        }

        [Test]
        public void SelectJoinsWithOnWithMaps()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                // multiple joins using On<T> with include
                var orders = context
                    .From<Orders>()
                    .Join<OrderDetails>((det, order) => det.OrdersID == order.OrdersID)
                    .Join<Products>((product, det) => product.ProductID == det.ProductID)
                    .Map(p => p.ProductID)
                    .Map(p => p.UnitPrice)
                    .Select<OrderWithDetail>();

                /* *Expected Query*
                 select Products.ProductID, Products.UnitPrice, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, Quantity, Discount 
                 from Orders 
                 join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)
                 join Products on (Products.ProductID = OrderDetails.ProductID)
                */

                Assert.IsTrue(orders.Any());
                Assert.IsFalse(string.IsNullOrEmpty(orders.First().ShipName));
                Assert.IsTrue(orders.First().ProductID > 0);
            }
        }

        [Test]
        [Description("A select statemenent with a join and a or operation")]
        public void SelectJoinWithGenericOr()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // join using on and or
                var query = context.From<Employee>()
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Or<Employee>((c, e) => c.EmployeeID == e.EmployeeID)
                    .Map(o => o.Region)
                    .Map(o => o.CustomerID)
                    .Map(o => o.Country);

                // execute the query
                var customers = query.Select<Customers>();

                var sql = query.CompileQuery<Customers>().Flatten();
                var expected = "select Employee.EmployeeID, Employee.Address, Employee.City, Employee.PostalCode, Customers.Region, Customers.CustomerID, Customers.Country, CompanyName, ContactName, ContactTitle, Phone, Fax from Employee join Orders on (Orders.EmployeeID = Employee.EmployeeID) join Customers on (Customers.CustomerID = Orders.CustomerID) or (Customers.EmployeeID = Employee.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(customers.Any());
            }
        }

        [Test]
        [Description("select with a join that connects to a other table than the previous")]
        public void SelectJoinToOtherTable()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // select with a join that connects to a other table than the previous
                var query = context.From<Employee>()
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                    .Join<Customers, Employee>((c, e) => c.EmployeeID == e.EmployeeID);

                // execute the query
                var customers = query.Select<Employee>();

                var sql = query.CompileQuery<Employee>().Flatten();
                var expected = "select Employee.EmployeeID, Employee.Address, Employee.City, Employee.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Employee join Orders on (Orders.EmployeeID = Employee.EmployeeID) join Customers on (Customers.EmployeeID = Employee.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(customers.Any());
            }
        }

        [Test]
        [Description("select with a join that contains a alias connects to a other table than the previous")]
        public void SelectJoinWithAliasToOtherTable()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // select with a join that contains a alias connects to a other table than the previous
                var query = context.From<Employee>()
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                    .Join<Customers, Employee>((c, e) => c.EmployeeID == e.EmployeeID, "cust");

                // execute the query
                var customers = query.Select<Employee>();

                var sql = query.CompileQuery<Employee>().Flatten();
                var expected = "select Employee.EmployeeID, Employee.Address, Employee.City, Employee.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Employee join Orders on (Orders.EmployeeID = Employee.EmployeeID) join Customers cust on (cust.EmployeeID = Employee.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(customers.Any());
            }
        }

        [Test]
        [Description("select with a join that containes an alias and connects to a other table than the previous that also contains an alias")]
        public void SelectJoinWithAliasToOtherTableWithAlias()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // select with a join that containes an alias and connects to a other table than the previous that also contains an alias
                var query = context.From<Employee>("emp")
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID, source: "emp")
                    .Join<Customers, Employee>((c, e) => c.EmployeeID == e.EmployeeID, "cust", "emp");

                // execute the query
                var customers = query.Select<Employee>();

                var sql = query.CompileQuery<Employee>().Flatten();
                var expected = "select emp.EmployeeID, emp.Address, emp.City, emp.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Employee emp join Orders on (Orders.EmployeeID = emp.EmployeeID) join Customers cust on (cust.EmployeeID = emp.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(customers.Any());
            }
        }

        [Test]
        [Description("select expression containing a and with an alias for the source table")]
        public void SelectJoinWithAndWithOnlySourceAliasWithProjection()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                //TODO: And allways returns false! create provider that realy works!
                var query = context.From<Customers>()
                    .Join<Orders>((o, c) => o.EmployeeID == c.EmployeeID, "ord")
                    .Join<Employee>((e, o) => e.EmployeeID == o.EmployeeID, "empl", "ord")
                    .And<Customers>((e, c) => e.EmployeeID == c.EmployeeID, alias: "empl")
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode);

                var sql = query.CompileQuery<Employee>().Flatten();
                var expected = "select empl.EmployeeID, empl.Address, empl.City, empl.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Customers join Orders ord on (ord.EmployeeID = Customers.EmployeeID) join Employee empl on (empl.EmployeeID = ord.EmployeeID) and (empl.EmployeeID = Customers.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                // execute the query
                var employees = query.Select<Employee>();

                Assert.IsTrue(employees.Any());
            }
        }

        [Test]
        [Description("select expression containing a and with an alias for the joined table")]
        public void SelectJoinWithAndWithOnlyAliasWithProjection()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                //TODO: And allways returns false! create provider that realy works!
                var query = context.From<Customers>("cust")
                    .Join<Orders>((o, c) => o.EmployeeID == c.EmployeeID, "ord", "cust")
                    .Join<Employee>((e, o) => e.EmployeeID == o.EmployeeID, source: "ord")
                    .And<Customers>((e, c) => e.EmployeeID == c.EmployeeID, source: "cust")
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode);

                var sql = query.CompileQuery<Employee>().Flatten();
                var expected = "select Employee.EmployeeID, Employee.Address, Employee.City, Employee.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Customers cust join Orders ord on (ord.EmployeeID = cust.EmployeeID) join Employee on (Employee.EmployeeID = ord.EmployeeID) and (Employee.EmployeeID = cust.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                // execute the query
                var employees = query.Select<Employee>();

                Assert.IsTrue(employees.Any());
            }
        }






        [Test]
        public void SelectWithMapToInJoinWithTypeSourceAndStringAlias()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // Map => To in join with string
                var query = context.From<Orders>()
                    // map the property from this join to the Property in the result type
                    .Map(source => source.Freight, "SpecialFreight")
                    .Join<OrderDetails>((detail, order) => detail.OrdersID == order.OrdersID)
                    .Map(i => i.OrdersID);

                var sql = "select Orders.Freight as SpecialFreight, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

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
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // Map => To in join with predicate
                var query = context.From<Orders>()
                    // map the property from this join to the Property in the result type
                    .Map<OrderWithDetailExtended>(source => source.Freight, alias => alias.SpecialFreight)
                    .Join<OrderDetails>((detail, order) => detail.OrdersID == order.OrdersID)
                    .Map(i => i.OrdersID);

                var sql = "select Orders.Freight as SpecialFreight, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderWithDetailExtended>().Flatten(), sql);

                // execute the query
                var orders = query.Select<OrderWithDetailExtended>();

                Assert.IsTrue(orders.Any());
            }
        }

        //[Test]
        //public void SelectWithMultipleMapsToSameType()
        //{
        //    var sql = "";
        //    var provider = new CallbackContextProvider(s => sql = s.Flatten());
        //    using (var context = provider.Open())
        //    {
        //        // select the properties that are defined in the mapping
        //        context.From<WarriorWithName>()
        //            .Map(w => w.WeaponID, "ID")
        //            .Map(w => w.WeaponID)
        //            .Map(w => w.Race, "Name")
        //            .Map(w => w.Race)
        //            .Select();

        //        Assert.AreEqual(sql, "select WarriorWithName.WeaponID as ID, WarriorWithName.WeaponID, WarriorWithName.Race as Name, WarriorWithName.Race, SpecialSkill from WarriorWithName");

        //        // map one property to a custom field
        //        context.From<WarriorWithName>()
        //            .Map(w => w.WeaponID, "ID")
        //            .Map(w => w.Race, "Name")
        //            .Map(w => w.Race)
        //            .Select();

        //        Assert.AreEqual(sql, "select WarriorWithName.WeaponID as ID, WarriorWithName.Race as Name, WarriorWithName.Race, SpecialSkill from WarriorWithName");
        //    }
        //}

        //[Test]
        //public void ISelectQueryExpressionWithIgnoringFields()
        //{
        //    var sql = "";
        //    var provider = new CallbackContextProvider(s => sql = s.Flatten());
        //    using (var context = provider.Open())
        //    {
        //        // ignore a member in the select
        //        context.From<WarriorWithName>()
        //            .Ignore(w => w.ID)
        //            .Ignore(w => w.Name)
        //            .Ignore(w => w.SpecialSkill)
        //            .Select();

        //        Assert.AreEqual(sql, "select WeaponID, Race from WarriorWithName");

        //        // ignore a member in the select
        //        context.From<WarriorWithName>()
        //            .Ignore(w => w.ID)
        //            .Ignore(w => w.Name)
        //            .Ignore(w => w.SpecialSkill)
        //            .Map(w => w.Name)
        //            .Select();

        //        Assert.AreEqual(sql, "select WarriorWithName.Name, WeaponID, Race from WarriorWithName");

        //        // ignore a member in the select
        //        context.From<WarriorWithName>()
        //            .Ignore(w => w.ID)
        //            .Ignore(w => w.Name)
        //            .Map(w => w.WeaponID, "TestFieldName")
        //            .Ignore(w => w.SpecialSkill)
        //            .Select();

        //        Assert.AreEqual(sql, "select WarriorWithName.WeaponID as TestFieldName, Race from WarriorWithName");
        //    }
        //}


        [Test]
        [Description("select statement with where operation for generic type")]
        public void SelectWhereForGenericType()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // where operation on new generic type
                var query = context.From<Orders>()
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((d, o) => d.OrdersID == o.OrdersID)
                    .Where<Orders>(o => o.Freight > 0);

                // execute the query
                var orders = query.Select<OrderDetails>();

                var sql = query.CompileQuery<OrderDetails>().Flatten();
                var expected = "select Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID) where (Orders.Freight > '0')";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("select statement with where operation for multiple generic types")]
        public void SelectWhereForMultipeGnenericTypes()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // where operation on new generic type
                var query = context.From<Employee>()
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Where<Employee, Customers>((e, c) => e.EmployeeID != c.EmployeeID);

                // execute the query
                var employees = query.Select<Employee>();

                var sql = query.CompileQuery<Employee>().Flatten();
                var expected = "select Employee.EmployeeID, Employee.Address, Employee.City, Employee.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Employee join Orders on (Orders.EmployeeID = Employee.EmployeeID) join Customers on (Customers.CustomerID = Orders.CustomerID) where (Employee.EmployeeID <> Customers.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(employees.Any());
            }
        }

        [Test]
        [Description("select statement with where operation and a simple generic and operation")]
        public void SelectWhereWithGenericAnd()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // where operation with and
                var query = context.From<Orders>()
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((d, o) => d.OrdersID == o.OrdersID)
                    .Where<Orders, OrderDetails>((o, od) => o.OrdersID == od.OrdersID)
                    .And<OrderDetails>((od) => od.Quantity > 10);

                // execute the query
                var orderdetails = query.Select();

                var sql = query.CompileQuery<OrderDetails>().Flatten();
                var expected = "select Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID) where (Orders.OrdersID = OrderDetails.OrdersID) and (OrderDetails.Quantity > 10)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orderdetails.Any());
            }
        }

        [Test]
        [Description("select statement with a where operationion and a and operation mapped to 2 differen types")]
        public void SelectWhereWithMultyGenericAnd()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // where operation with and
                var query = context.From<Employee>()
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                    .Map(o => o.EmployeeID)
                    .Map(o => o.CustomerID)
                    .Join<Customers>((c, o) => o.CustomerID == o.CustomerID)
                    .Where(c => c.ContactName.Contains("a"))
                    .And<Customers, Employee>((c, e) => c.EmployeeID == e.EmployeeID);

                // execute the query
                var orders = query.Select<Orders>();

                var sql = query.CompileQuery<Orders>().Flatten();
                var expected = "select Orders.EmployeeID, Orders.CustomerID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Employee join Orders on (Orders.EmployeeID = Employee.EmployeeID) join Customers on (Orders.CustomerID = Orders.CustomerID) where Customers.ContactName like '%a%' and (Customers.EmployeeID = Employee.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("select statement with a where and a simple or operation")]
        public void SelectWhereWithSimpleGenericOr2()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // join using on and or
                var query = context.From<Employee>()
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(o => o.Region)
                    .Map(o => o.CustomerID)
                    .Map(o => o.Country)
                    .Where<Employee, Customers>((e, o) => e.EmployeeID == o.EmployeeID)
                    .Or<Orders>(o => o.Freight > 0);

                // execute the query
                var orders = query.Select<Customers>();

                var sql = query.CompileQuery<Customers>().Flatten();
                var expected = "select Employee.EmployeeID, Employee.Address, Employee.City, Employee.PostalCode, Customers.Region, Customers.CustomerID, Customers.Country, CompanyName, ContactName, ContactTitle, Phone, Fax from Employee join Orders on (Orders.EmployeeID = Employee.EmployeeID) join Customers on (Customers.CustomerID = Orders.CustomerID) where (Employee.EmployeeID = Customers.EmployeeID) or (Orders.Freight > '0')";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("select statement with a where and a or operation containing two entities")]
        public void SelectWhereWithSimpleComplexGenericOr()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // join using on and or
                var query = context.From<Employee>()
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(o => o.Region)
                    .Map(o => o.CustomerID)
                    .Map(o => o.Country)
                    .Where<Employee, Customers>((e, o) => e.EmployeeID == o.EmployeeID)
                    .Or<Employee>((c, e) => c.EmployeeID == e.EmployeeID);

                // execute the query
                var orders = query.Select<Customers>();

                var sql = query.CompileQuery<Customers>().Flatten();
                var expected = "select Employee.EmployeeID, Employee.Address, Employee.City, Employee.PostalCode, Customers.Region, Customers.CustomerID, Customers.Country, CompanyName, ContactName, ContactTitle, Phone, Fax from Employee join Orders on (Orders.EmployeeID = Employee.EmployeeID) join Customers on (Customers.CustomerID = Orders.CustomerID) where (Employee.EmployeeID = Customers.EmployeeID) or (Customers.EmployeeID = Employee.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("select statement with a where operation and a or operation that has two genereic parameters")]
        public void SelectWhereWithComplexGenericOr()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // join using on and and
                var query = context.From<Customers>()
                    .Map(e => e.EmployeeID)
                    .Map(e => e.Address)
                    .Map(e => e.City)
                    .Map(e => e.PostalCode)
                    .Join<Orders>((o, c) => o.EmployeeID == c.EmployeeID)
                    .Join<Employee>((e, o) => e.EmployeeID == o.EmployeeID)
                    .Where(e => e.FirstName.Contains("Davolio"))
                    .Or<Customers, Employee>((c, e) => c.EmployeeID == e.EmployeeID);

                // execute the query
                var orders = query.Select<Employee>();

                var sql = query.CompileQuery<Employee>().Flatten();
                var expected = "select Customers.EmployeeID, Customers.Address, Customers.City, Customers.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Customers join Orders on (Orders.EmployeeID = Customers.EmployeeID) join Employee on (Employee.EmployeeID = Orders.EmployeeID) where Employee.FirstName like '%Davolio%' or (Customers.EmployeeID = Employee.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("select statement with a where operationion and a and operation mapped to 2 differen types and containing an alias")]
        public void SelectWhereWithMultyGenericAndWithAlias()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // where operation with and
                var query = context.From<Employee>()
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                    .Map(o => o.EmployeeID)
                    .Map(o => o.CustomerID)
                    .Join<Customers>((c, o) => o.CustomerID == o.CustomerID, alias: "cust")
                    .Where(c => c.ContactName.Contains("a"))
                    .And<Customers, Employee>((c, e) => c.EmployeeID == e.EmployeeID, alias: "cust");

                // execute the query
                var orders = query.Select<Orders>();

                var sql = query.CompileQuery<Orders>().Flatten();
                var expected = "select Orders.EmployeeID, Orders.CustomerID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Employee join Orders on (Orders.EmployeeID = Employee.EmployeeID) join Customers cust on (Orders.CustomerID = Orders.CustomerID) where cust.ContactName like '%a%' and (cust.EmployeeID = Employee.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        [Description("select statement with a where operationion and a and operation mapped to 2 differen types and containing an alias for the source")]
        public void SelectWhereWithMultyGenericAndWithAliasForSource()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // where operation with and
                var query = context.From<Employee>("emp")
                    .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID, source: "emp")
                    .Map(o => o.EmployeeID)
                    .Map(o => o.CustomerID)
                    .Join<Customers>((c, o) => o.CustomerID == o.CustomerID)
                    .Where(c => c.ContactName.Contains("a"))
                    .And<Customers, Employee>((c, e) => c.EmployeeID == e.EmployeeID, source: "emp");

                // execute the query
                var orders = query.Select<Orders>();

                var sql = query.CompileQuery<Orders>().Flatten();
                var expected = "select Orders.EmployeeID, Orders.CustomerID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Employee emp join Orders on (Orders.EmployeeID = emp.EmployeeID) join Customers on (Orders.CustomerID = Orders.CustomerID) where Customers.ContactName like '%a%' and (Customers.EmployeeID = emp.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(orders.Any());
            }
        }

    }
}