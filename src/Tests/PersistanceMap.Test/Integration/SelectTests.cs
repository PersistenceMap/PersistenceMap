using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;
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
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
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
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var query = context.From<Orders>()
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((d, o) => d.OrdersID == o.OrdersID);

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
        public void SelectWithMapping()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // Map => To 
                var query = context.From<Orders>()
                    .Map<OrderWithDetailExtended>(source => source.Freight, alias => alias.SpecialFreight)
                    .Join<OrderDetails>((detail, order) => detail.OrdersID == order.OrdersID)
                    // map a property from a joni to a property in the result type
                    .Map(i => i.OrdersID);

                var sql = "select Orders.Freight as SpecialFreight, OrderDetails.OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderWithDetailExtended>().Flatten(), sql);

                // execute the query
                var orders = query.Select<OrderWithDetailExtended>();

                Assert.IsTrue(orders.Any());
                Assert.IsTrue(orders.First().SpecialFreight > 0);
            }
        }

        [Test]
        public void SelectWithExtendedMapping()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // Map => To 
                var query = context.From<Orders>()
                    .Join<OrderDetails>((detail, order) => detail.OrdersID == order.OrdersID)
                    .Map(i => i.OrdersID)
                    // map a property from a joni to a property in the result type
                    .Map<Orders, OrderWithDetailExtended>(source => source.Freight, alias => alias.SpecialFreight);

                var sql = "select OrderDetails.OrdersID, Orders.Freight as SpecialFreight, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderWithDetailExtended>().Flatten(), sql);

                // execute the query
                var orders = query.Select<OrderWithDetailExtended>();
                
                Assert.IsTrue(orders.Any());
                Assert.IsTrue(orders.First().SpecialFreight > 0);
            }
        }

        [Test]
        public void Select_WithStringSelectStatement()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // select with string select statement
                var orders = context.Select<Orders>("Select * from Orders");

                Assert.IsTrue(orders.Any());
            }
        }


        [Test(Description = "Select with a anonym object definition")]
        public void Select_Anonym_ObjectTypeDefiniton()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                var anonymous = context.From<Orders>()
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)
                    .Select(() => new
                    {
                        ProductID = 0,
                        Quantity = 0.0
                    });

                Assert.IsTrue(anonymous.Any());
                Assert.IsTrue(anonymous.First().ProductID > 0);
                Assert.IsTrue(anonymous.First().Quantity > 0);
            }
        }

        [Test(Description = "Select to a anonym object")]
        public void Select_Anonym_Object()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                var query = context.From<Orders>()
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID);

                var anonymous = query.Select(od => new
                {
                    ProductID = od.ProductID,
                    Quantity = od.Quantity
                });

                var sql = query.CompileQuery<OrderDetails>().Flatten();
                var expected = "select Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

                Assert.AreEqual(sql, expected);
                Assert.IsTrue(anonymous.Any());
                Assert.IsTrue(anonymous.First().ProductID > 0);
                Assert.IsTrue(anonymous.First().Quantity > 0);
            }
        }

        [Test(Description = "Select to a anonym object delegate")]
        public void Select_Anonym_Object2()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                var query = context.From<Orders>()
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID);

                // select into a anonymous object
                var anonymous = query.Select(od => new
                {
                    Prud = od.Quantity
                });

                var sql = query.CompileQuery<OrderDetails>().Flatten();
                var expected = "select Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

                Assert.AreEqual(sql, expected); 
                Assert.IsTrue(anonymous.Any());
                Assert.IsTrue(anonymous.First().Prud > 0);

            }
        }

        [Test(Description = "Select to a type object delegate")]
        public void Select_Object_Delegate()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // select only the properties that are defined in the anony object
                var query = context.From<Orders>()
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID);

                var orders = query.Select(od => new OrderWithDetail
                {
                    // only select the properties defined
                    ProductID = od.ProductID,
                    Quantity = od.Quantity
                });

                var sql = query.CompileQuery<OrderDetails>().Flatten();
                var expected = "select Orders.OrdersID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

                Assert.AreEqual(sql, expected); 
                Assert.IsTrue(orders.Any());
                Assert.IsTrue(orders.First().ProductID > 0);
                Assert.IsTrue(orders.First().Quantity > 0);
                Assert.IsTrue(orders.First().CustomerID == null);
            }
        }

        [Test]
        [Description("select statement that compiles from a FOR operation with a anonym object defining the resultset entries")]
        public void Select_For_Anonym_ObjectType()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // select only the properties that are defined in the anony object
                var query = context.From<Orders>()
                    .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)
                    .For(() => new
                    {
                        ProductID = 0,
                        Quantity = 0
                    });

                var anonymous = query.Select();

                var sql = query.CompileQuery().Flatten();
                var expected = "select ProductID, Quantity from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

                Assert.AreEqual(sql, expected);

                Assert.IsTrue(anonymous.Any());
                Assert.IsTrue(anonymous.First().ProductID > 0);
                Assert.IsTrue(anonymous.First().Quantity > 0);
            }
        }

        [Test]
        [Description("select statement that compiles from a FOR operation with a anonym object defining the resultset entries and mapped to a defined type")]
        public void Select_For_Anonym_To_DefinedType()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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
        public void Select_For_Anonym_CastTo_DefinedType_Delegate()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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




        [Test, TestCaseSource(typeof(SelectTestCases), "SelectTestCasesForOrders")]
        public string SelectTestForOrders(IOrderQueryProvider<Orders> query)
        {
            // execute the query
            var orders = query.Select();

            Assert.IsTrue(orders.Any());

            // return the query string
            return query.CompileQuery<Orders>().Flatten();
        }
    }

    internal class SelectTestCases : TestBase
    {
        public IEnumerable SelectTestCasesForOrders
        {
            get
            {
                var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
                using (var context = connection.Open())
                {
                    yield return new TestCaseData(context.From<Orders>()
                        .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)
                        .For<Orders>()
                        .Map<Orders>(o => o.OrdersID))
                        .Returns("select Orders.OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)")
                        .SetDescription("select statement with a FOR expression and mapping members/fields to a specific table")
                        .SetName("select statement with a FOR expression and mappings");

                    yield return new TestCaseData(context.From<Orders>()
                        .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)
                        .For<Orders>()
                        .Ignore(o => o.OrdersID)
                        .Ignore(o => o.OrderDate)
                        .Ignore(o => o.RequiredDate))
                        .Returns("select CustomerID, EmployeeID, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)")
                        .SetDescription("select statement with a FOR expression and ignoring fields in the resultset")
                        .SetName("select statement with a FOR expression and ignoring fields in the resultset");

                    yield return new TestCaseData(context.From<Orders>()
                        .Map(o => o.OrdersID)
                        .Join<OrderDetails>((od, o) => od.OrdersID == o.OrdersID)
                        .For<Orders>())
                        .Returns("select Orders.OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)")
                        .SetDescription("select statement that compiles from a FOR operation with a anonym object defining the resultset entries and mapped to a defined type")
                        .SetName("select statement that compiles from a FOR operation with a anonym object defining the resultset entries and mapped to a defined type");

                    yield return new TestCaseData(context.From<Orders>())
                        .Returns("select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders")
                        .SetDescription("simple select from statement")
                        .SetName("simple select from statement");

                    /*
                    yield return new TestCaseData()
                        .Returns("")
                        .SetDescription("")
                        .SetName("");
                     */
                }
            }
        }
    }

    [TestFixture]
    public class FromTests : TestBase
    {
        [Test]
        public void Select_From_Direct()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
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
        public void Select_From_WithAliasInFrom()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
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
        public void Select_FromW_ithJoin_WithAlias()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
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
        public void Select_From_WithAlias()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
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
        public void From_WithInclude_WithAlias_MapWithoutAlias()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
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
        [ExpectedException(typeof(SqlException))]
        public void IncludeWithWrongLambdaExpressionFailTest()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // fail test because Include doesn't return a property witch ends in a wrong sql statement
                var tmp = context.From<Orders>()
                    .Join<OrderDetails>((detail, order) => detail.OrdersID == order.OrdersID)
                    // this has to fail!
                    .Map(i => i.OrdersID != 1)
                    .Select<OrderWithDetailExtended>();

                Assert.Fail("This part should not have been reached!");
            }
        }

        [Test]
        public void JoinWithIndexerInMember()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {

                // product with indexer (this[string])
                var products = context.From<Products>().Select<ProductsWithIndexer>();

                Assert.IsTrue(products.Any());
            }
        }
    }

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
                    .Join<OrderDetails>((det, order) => det.OrdersID == order.OrdersID)
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
        public void Select_Join_WithOn_WithProjection()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
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
        public void Select_Join_WithAlias_WithMaps_WithProjection()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
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
        public void Select_Join_WithAliasInFrom_WithMaps_WithProjection()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var query = context.From<Orders>("orders")
                    .Map(o => o.OrdersID)
                    .Join<OrderDetails>((detail, order) => detail.OrdersID == order.OrdersID, "detail", "orders");

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderWithDetail>().Flatten(), "select orders.OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders orders join OrderDetails detail on (detail.OrdersID = orders.OrdersID)");

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
        public void Select_Join_WithAnd_WithProjection()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
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
        public void Select_Join_WithAnd_WithAlias_WithProjection()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                //TODO: And allways returns false! create connection that realy works!
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
                var Employee = query.Select<Employee>();

                Assert.IsTrue(Employee.Any());
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
        public void Select_Joins_WithOn_WithMaps()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
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
        public void Select_Join_WithGenericOr()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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
        public void Select_Join_ToOtherTable()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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
        public void Select_Join_WithAlias_ToOtherTable()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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
        public void Select_Join_WithAlias_ToOtherTable_WithAlias()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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
        public void Select_Join_WithAnd_WithOnlySourceAlias_WithProjection()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                //TODO: And allways returns false! create connection that realy works!
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
                var Employee = query.Select<Employee>();

                Assert.IsTrue(Employee.Any());
            }
        }

        [Test]
        [Description("select expression containing a and with an alias for the joined table")]
        public void Select_Join_WithAnd_WithOnlyAlias_WithProjection()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                //TODO: And allways returns false! create connection that realy works!
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
                var Employee = query.Select<Employee>();

                Assert.IsTrue(Employee.Any());
            }
        }
    }

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
                    .Join<OrderDetails>((detail, order) => detail.OrdersID == order.OrdersID)
                    .Map(i => i.OrdersID);

                var sql = "select Orders.Freight as SpecialFreight, OrderDetails.OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

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
                    .Map<OrderWithDetailExtended>(source => source.Freight, alias => alias.SpecialFreight)
                    .Join<OrderDetails>((detail, order) => detail.OrdersID == order.OrdersID)
                    .Map(i => i.OrdersID);

                var sql = "select Orders.Freight as SpecialFreight, OrderDetails.OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID)";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderWithDetailExtended>().Flatten(), sql);

                // execute the query
                var orders = query.Select<OrderWithDetailExtended>();

                Assert.IsTrue(orders.Any());
            }
        }
    }

    [TestFixture]
    public class OrderTests
    {
        [Test, TestCaseSource(typeof(OrderTestCases), "TestCases")]
        public string OrderTest(IOrderQueryProvider<Orders> query)
        {
            // execute the query
            var orders = query.Select();

            Assert.IsTrue(orders.Any());

            // return the query string
            return query.CompileQuery<Orders>().Flatten();
        }

        class OrderTestCases : TestBase
        {
            public IEnumerable TestCases
            {
                get
                {
                    var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
                    using (var context = connection.Open())
                    {
                        yield return new TestCaseData(context.From<Orders>()
                            .OrderBy(o => o.OrderDate))
                            .Returns("select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate asc")
                            .SetName("join with simple order by")
                            .SetDescription("join with simple order by");

                        yield return new TestCaseData(context.From<Orders>()
                            .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                            .Map(c => c.CustomerID)
                            .Map(c => c.EmployeeID)
                            .OrderBy<Orders>(o => o.OrderDate))
                            .Returns("select Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate asc")
                            .SetName("join with generic order by")
                            .SetDescription("join with generic order by");

                        yield return new TestCaseData(context.From<Orders>()
                            .OrderByDesc(o => o.OrderDate))
                            .Returns("select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate desc")
                            .SetName("join with simple order by desc")
                            .SetDescription("join with simple order by desc");

                        yield return new TestCaseData(context.From<Orders>()
                            .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                            .Map(c => c.CustomerID)
                            .Map(c => c.EmployeeID)
                            .OrderByDesc<Orders>(o => o.OrderDate))
                            .Returns("select Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate desc")
                            .SetName("join with generic order by desc")
                            .SetDescription("join with generic order by desc");

                        yield return new TestCaseData(context.From<Orders>()
                            .OrderBy(o => o.OrderDate)
                            .ThenBy(o => o.RequiredDate))
                            .Returns("select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate asc , Orders.RequiredDate asc")
                            .SetName("join with simple order by with simple then by")
                            .SetDescription("join with simple order by with simple then by");

                        yield return new TestCaseData(context.From<Orders>()
                            .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                            .Map(c => c.CustomerID)
                            .Map(c => c.EmployeeID)
                            .OrderBy<Orders>(o => o.OrderDate)
                            .ThenBy(o => o.RequiredDate))
                            .Returns("select Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate asc , Orders.RequiredDate asc")
                            .SetName("join with generic order by with simple then by")
                            .SetDescription("join with generic order by with simple then by");

                        yield return new TestCaseData(context.From<Orders>()
                            .OrderByDesc(o => o.OrderDate)
                            .ThenBy(o => o.RequiredDate))
                            .Returns("select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate desc , Orders.RequiredDate asc")
                            .SetName("join with simple order by desc with simple then by")
                            .SetDescription("join with simple order by desc with simple then by");

                        yield return new TestCaseData(context.From<Orders>()
                            .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                            .Map(c => c.CustomerID)
                            .Map(c => c.EmployeeID)
                            .OrderByDesc<Orders>(o => o.OrderDate)
                            .ThenBy(o => o.RequiredDate))
                            .Returns("select Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate desc , Orders.RequiredDate asc")
                            .SetName("join with generic order by desc with simple then by")
                            .SetDescription("join with generic order by desc with simple then by");

                        yield return new TestCaseData(context.From<Orders>()
                            .OrderBy(o => o.OrderDate)
                            .ThenBy<Orders>(o => o.RequiredDate))
                            .Returns("select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate asc , Orders.RequiredDate asc")
                            .SetName("join with simple order by with generic then by")
                            .SetDescription("join with simple order by with generic then by");

                        yield return new TestCaseData(context.From<Orders>()
                            .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                            .Map(c => c.CustomerID)
                            .Map(c => c.EmployeeID)
                            .OrderBy<Orders>(o => o.OrderDate)
                            .ThenBy<Customers>(c => c.CompanyName))
                            .Returns("select Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate asc , Customers.CompanyName asc")
                            .SetName("join with generic order by with generic then by")
                            .SetDescription("join with generic order by with generic then by");

                        yield return new TestCaseData(context.From<Orders>()
                            .OrderByDesc(o => o.OrderDate)
                            .ThenBy<Orders>(o => o.RequiredDate))
                            .Returns("select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate desc , Orders.RequiredDate asc")
                            .SetName("join with simple order by desc with generic then by")
                            .SetDescription("join with simple order by desc with generic then by");

                        yield return new TestCaseData(context.From<Orders>()
                            .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                            .Map(c => c.CustomerID)
                            .Map(c => c.EmployeeID)
                            .OrderByDesc<Orders>(o => o.OrderDate)
                            .ThenBy<Customers>(c => c.CompanyName))
                            .Returns("select Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate desc , Customers.CompanyName asc")
                            .SetName("join with generic order by desc with generic then by")
                            .SetDescription("join with generic order by desc with generic then by");

                        yield return new TestCaseData(context.From<Orders>()
                            .OrderBy(o => o.OrderDate)
                            .ThenByDesc<Orders>(o => o.RequiredDate))
                            .Returns("select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate asc , Orders.RequiredDate desc")
                            .SetName("join with simple order by with generic then by desc")
                            .SetDescription("join with simple order by with generic then by desc");

                        yield return new TestCaseData(context.From<Orders>()
                            .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                            .Map(c => c.CustomerID)
                            .Map(c => c.EmployeeID)
                            .OrderBy<Orders>(o => o.OrderDate)
                            .ThenByDesc<Customers>(c => c.CompanyName))
                            .Returns("select Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate asc , Customers.CompanyName desc")
                            .SetName("join with generic order by with generic then by desc")
                            .SetDescription("join with generic order by with generic then by desc");

                        yield return new TestCaseData(context.From<Orders>()
                            .OrderByDesc(o => o.OrderDate)
                            .ThenByDesc<Orders>(o => o.RequiredDate))
                            .Returns("select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate desc , Orders.RequiredDate desc")
                            .SetName("join with simple order by desc with generic then by desc")
                            .SetDescription("join with simple order by desc with generic then by desc");

                        yield return new TestCaseData(context.From<Orders>()
                            .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                            .Map(c => c.CustomerID)
                            .Map(c => c.EmployeeID)
                            .OrderByDesc<Orders>(o => o.OrderDate)
                            .ThenByDesc<Customers>(c => c.CompanyName))
                            .Returns("select Customers.CustomerID, Customers.EmployeeID, OrdersID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate desc , Customers.CompanyName desc")
                            .SetName("join with generic order by desc with generic then by desc")
                            .SetDescription("join with generic order by desc with generic then by desc");
                    }
                }
            }
        }
    }

    [TestFixture]
    public class WhereTests : TestBase
    {
        [Test]
        [Description("select statement with where operation for generic type")]
        public void Select_WhereForGenericType()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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
        public void Select_WhereForMultipeGnenericTypes()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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
                var Employee = query.Select<Employee>();

                var sql = query.CompileQuery<Employee>().Flatten();
                var expected = "select Employee.EmployeeID, Employee.Address, Employee.City, Employee.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo from Employee join Orders on (Orders.EmployeeID = Employee.EmployeeID) join Customers on (Customers.CustomerID = Orders.CustomerID) where (Employee.EmployeeID <> Customers.EmployeeID)";

                // check the compiled sql
                Assert.AreEqual(sql, expected);

                Assert.IsTrue(Employee.Any());
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
        public void Select_Where_WithMultyGenericAnd()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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
        public void Select_Where_WithSimpleGenericOr2()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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
        public void Select_Where_WithSimpleComplexGenericOr()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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
        public void Select_Where_WithComplexGenericOr()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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
        public void Select_Where_WithMultyGenericAnd_WithAlias()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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
        public void Select_Where_WithMultyGenericAnd_WithAliasForSource()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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



        [Test, TestCaseSource(typeof(SelectWhereTestCases), "EmployeeTestCases")]
        public string WhereTest(IOrderQueryProvider<Employee> query)
        {
            // execute the query
            var orders = query.Select();

            Assert.IsTrue(orders.Any());

            // return the query string
            return query.CompileQuery<Orders>().Flatten();
        }

        [Test, TestCaseSource(typeof(SelectWhereTestCases), "OrdersTestCases")]
        public string WhereTest(IOrderQueryProvider<Orders> query)
        {
            // execute the query
            var orders = query.Select();

            Assert.IsTrue(orders.Any());

            // return the query string
            return query.CompileQuery<Orders>().Flatten();
        }
    }

    class SelectWhereTestCases : TestBase
    {
        #region EmployeeTestCases

        public IEnumerable EmployeeTestCases
        {
            get
            {
                var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
                using (var context = connection.Open())
                {
                    yield return new TestCaseData(context.From<Customers>("cust")
                        .Map(e => e.EmployeeID)
                        .Map(e => e.Address)
                        .Map(e => e.City)
                        .Map(e => e.PostalCode)
                        .Join<Orders>((o, c) => o.EmployeeID == c.EmployeeID, source: "cust")
                        .Join<Employee>((e, o) => e.EmployeeID == o.EmployeeID, alias: "emp")
                        .Where(e => e.FirstName.Contains("Davolio"))
                        .Or<Customers, Employee>((c, e) => c.EmployeeID == e.EmployeeID, "cust", "emp"))
                        .Returns("select cust.EmployeeID, cust.Address, cust.City, cust.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo, OrdersID, CustomerID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Customers cust join Orders on (Orders.EmployeeID = cust.EmployeeID) join Employee emp on (emp.EmployeeID = Orders.EmployeeID) where emp.FirstName like '%Davolio%' or (cust.EmployeeID = emp.EmployeeID)")
                        .SetDescription("select statement with a where operation and a or operation that has two genereic parameters and alias for both types")
                        .SetName("Where expression with Or containing aliases");

                    yield return new TestCaseData(context.From<Customers>()
                        .Map(e => e.EmployeeID)
                        .Map(e => e.Address)
                        .Map(e => e.City)
                        .Map(e => e.PostalCode)
                        .Join<Orders>((o, c) => o.EmployeeID == c.EmployeeID)
                        .Join<Employee>((e, o) => e.EmployeeID == o.EmployeeID, alias: "emp")
                        .Where(e => e.FirstName.Contains("Davolio"))
                        .Or<Customers, Employee>((c, e) => c.EmployeeID == e.EmployeeID, source: "emp"))
                        .Returns("select Customers.EmployeeID, Customers.Address, Customers.City, Customers.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo, OrdersID, CustomerID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Customers join Orders on (Orders.EmployeeID = Customers.EmployeeID) join Employee emp on (emp.EmployeeID = Orders.EmployeeID) where emp.FirstName like '%Davolio%' or (Customers.EmployeeID = emp.EmployeeID)")
                        .SetDescription("select statement with a where operation and a or operation that has two genereic parameters and a alias on the source type")
                        .SetName("Where expression with Or containing source alias");

                    yield return new TestCaseData(context.From<Customers>("cust")
                        .Map(e => e.EmployeeID)
                        .Map(e => e.Address)
                        .Map(e => e.City)
                        .Map(e => e.PostalCode)
                        .Join<Orders>((o, c) => o.EmployeeID == c.EmployeeID, source: "cust")
                        .Join<Employee>((e, o) => e.EmployeeID == o.EmployeeID)
                        .Where(e => e.FirstName.Contains("Davolio"))
                        .Or<Customers, Employee>((c, e) => c.EmployeeID == e.EmployeeID, alias: "cust"))
                        .Returns("select cust.EmployeeID, cust.Address, cust.City, cust.PostalCode, LastName, FirstName, Title, BirthDate, HireDate, ReportsTo, OrdersID, CustomerID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Customers cust join Orders on (Orders.EmployeeID = cust.EmployeeID) join Employee on (Employee.EmployeeID = Orders.EmployeeID) where Employee.FirstName like '%Davolio%' or (cust.EmployeeID = Employee.EmployeeID)")
                        .SetDescription("select statement with a where operation and a or operation that has two genereic parameters and a alias on the type")
                        .SetName("Where expression with Or containing aliase");
                }
            }
        }

        #endregion

        #region OrdersTestCases

        public IEnumerable OrdersTestCases
        {
            get
            {
                var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
                using (var context = connection.Open())
                {
                    yield return new TestCaseData(context.From<Orders>()
                        .Map(o => o.OrdersID)
                        .Join<OrderDetails>((d, o) => d.OrdersID == o.OrdersID)
                        .Where(o => o.Discount > 0)
                        .Rebase<OrderDetails, Orders>())
                        .Returns("select Orders.OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join OrderDetails on (OrderDetails.OrdersID = Orders.OrdersID) where (OrderDetails.Discount > '0')")
                        .SetDescription("Select statement with a simple where operation")
                        .SetName("Join expression with simple WHERE");

                    yield return new TestCaseData(context.From<Orders>()
                        .Where(p => p.CustomerID.StartsWith("P"))
                        .Or<Orders>(o => o.ShipCity == "London"))
                        .Returns("select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders where Orders.CustomerID like 'P%' or (Orders.ShipCity = 'London')")
                        .SetDescription("select statement with a where and a simple or operation")
                        .SetName("Where expression with generic OR");

                    yield return new TestCaseData(context.From<Orders>()
                        .Where(p => p.CustomerID.StartsWith("P"))
                        .Or<Orders>(o => o.ShipCity == "Paris")
                        .Or<Orders>(o => o.ShipCity == "London"))
                        .Returns("select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders where Orders.CustomerID like 'P%' or (Orders.ShipCity = 'Paris') or (Orders.ShipCity = 'London')")
                        .SetDescription("select statement with a where and a simple or operation")
                        .SetName("Where expression with two generic OR");

                    yield return new TestCaseData(context.From<Orders>("ord")
                        .Where(p => p.CustomerID.StartsWith("P"))
                        .Or<Orders>(o => o.ShipCity == "London", "ord"))
                        .Returns("select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders ord where ord.CustomerID like 'P%' or (ord.ShipCity = 'London')")
                        .SetDescription("Select statement with a where and a generic OR operation")
                        .SetName("Where expression with generic OR with alias");

                    yield return new TestCaseData(context.From<Orders>()
                        .Where(p => p.CustomerID.StartsWith("se"))
                        .And(o => o.ShipCity == "London"))
                        .Returns("select OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders where Orders.CustomerID like 'se%' and (Orders.ShipCity = 'London')")
                        .SetDescription("select statement with a where and a simple and operation")
                        .SetName("Where expression with simple And");

                }
            }
        }

        #endregion
    }
}
