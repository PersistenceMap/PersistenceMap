using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;
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
                select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry 
                from Orders
                */

                Assert.IsNotNull(orders);
                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        public void SimpleNongenericSelectTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var query = context.From<Orders>();

                var sql = "select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<Orders>().Flatten(), sql);

                // execute the query
                var orders = query.Select();

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
                    .Map(o => o.OrderID)
                    .Join<OrderDetails>((d, o) => d.OrderID == o.OrderID);

                var sql = "select Orders.OrderID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

                // check the compiled sql
                Assert.AreEqual(query.CompileQuery<OrderDetails>().Flatten(), sql);

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
                    .Map<OrderWithDetailExtended, double>(source => source.Freight, alias => alias.SpecialFreight)
                    .Join<OrderDetails>((detail, order) => detail.OrderID == order.OrderID)
                    // map a property from a joni to a property in the result type
                    .Map(i => i.OrderID);

                var sql = "select Orders.Freight as SpecialFreight, OrderDetails.OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

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
                    .Join<OrderDetails>((detail, order) => detail.OrderID == order.OrderID)
                    .Map(i => i.OrderID)
                    // map a property from a joni to a property in the result type
                    .Map<Orders, OrderWithDetailExtended, double>(source => source.Freight, alias => alias.SpecialFreight);

                var sql = "select OrderDetails.OrderID, Orders.Freight as SpecialFreight, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

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
                    .Map(o => o.OrderID)
                    .Join<OrderDetails>((od, o) => od.OrderID == o.OrderID)
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
                    .Map(o => o.OrderID)
                    .Join<OrderDetails>((od, o) => od.OrderID == o.OrderID);

                var anonymous = query.Select(od => new
                {
                    ProductID = od.ProductID,
                    Quantity = od.Quantity
                });

                var sql = query.CompileQuery<OrderDetails>().Flatten();
                var expected = "select Orders.OrderID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

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
                    .Map(o => o.OrderID)
                    .Join<OrderDetails>((od, o) => od.OrderID == o.OrderID);

                // select into a anonymous object
                var anonymous = query.Select(od => new
                {
                    Prud = od.Quantity
                });

                var sql = query.CompileQuery<OrderDetails>().Flatten();
                var expected = "select Orders.OrderID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

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
                    .Map(o => o.OrderID)
                    .Join<OrderDetails>((od, o) => od.OrderID == o.OrderID);

                var orders = query.Select(od => new OrderWithDetail
                {
                    // only select the properties defined
                    ProductID = od.ProductID,
                    Quantity = od.Quantity
                });

                var sql = query.CompileQuery<OrderDetails>().Flatten();
                var expected = "select Orders.OrderID, ProductID, UnitPrice, Quantity, Discount from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

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
                    .Join<OrderDetails>((od, o) => od.OrderID == o.OrderID)
                    .For(() => new
                    {
                        ProductID = 0,
                        Quantity = 0
                    });

                var anonymous = query.Select();

                var sql = query.CompileQuery().Flatten();
                var expected = "select ProductID, Quantity from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

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
                    .Join<OrderDetails>((od, o) => od.OrderID == o.OrderID)
                    .For(() => new
                    {
                        ProductID = 0,
                        Quantity = 0
                    });

                var anonymous = query.Select<OrderDetails>();

                var sql = query.CompileQuery().Flatten();
                var expected = "select ProductID, Quantity from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

                Assert.AreEqual(sql, expected);
                Assert.IsTrue(anonymous.Any());
                Assert.IsTrue(anonymous.First() is OrderDetails);
                Assert.IsTrue(anonymous.First().ProductID > 0);
                Assert.IsTrue(anonymous.First().Quantity > 0);
            }
        }

        [Test]
        [Description("select statement that compiles from a FOR operation with a anonym object defining the resultset entries and mapped to a defined type")]
        public void Select_For_DefinedType()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                var query = context.From<Orders>()
                    .Map(o => o.OrderID)
                    .Join<OrderDetails>((od, o) => od.OrderID == o.OrderID)
                    .For<Orders>();

                var anonymous = query.Select();

                var sql = query.CompileQuery().Flatten();
                var expected = "select Orders.OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

                Assert.AreEqual(sql, expected);
                Assert.IsTrue(anonymous.Any());
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
                    .Join<OrderDetails>((od, o) => od.OrderID == o.OrderID)
                    .For(() => new
                    {
                        ProductID = 0,
                        Quantity = 0
                    });

                var anonymous = query.Select(tmp => new OrderDetails { ProductID = tmp.ProductID, Quantity = tmp.Quantity });

                var sql = query.CompileQuery().Flatten();
                var expected = "select ProductID, Quantity from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

                Assert.AreEqual(sql, expected);
                Assert.IsTrue(anonymous.Any());
                Assert.IsTrue(anonymous.First() is OrderDetails);
                Assert.IsTrue(anonymous.First().ProductID > 0);
                Assert.IsTrue(anonymous.First().Quantity > 0);
            }
        }

        [Test]
        [Description("select statement with a FOR expression and ignoring fields in the resultset")]
        public void Select_For_DefinedType_IgnoreFields()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                var query = context.From<Orders>()
                    .Join<OrderDetails>((od, o) => od.OrderID == o.OrderID)
                    .For<Orders>()
                    .Ignore(o => o.OrderID)
                    .Ignore(o => o.OrderDate)
                    .Ignore(o => o.RequiredDate);

                var anonymous = query.Select();

                var sql = query.CompileQuery().Flatten();
                var expected = "select CustomerID, EmployeeID, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join OrderDetails on (OrderDetails.OrderID = Orders.OrderID)";

                Assert.AreEqual(sql, expected);
                Assert.IsTrue(anonymous.Any());
            }
        }
    }
}
